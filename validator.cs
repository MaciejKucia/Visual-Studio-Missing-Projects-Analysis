//-----------------------------------------------------------------------
// <copyright file="validator.cs" company="Maciej Kucia">
//     MIT License https://github.com/MaciejKucia/Visual-Studio-Missing-Projects-Analysis
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Microsoft.Build.Construction;

[assembly: AssemblyCopyright("Copyright © Maciej Kucia 2016")]
[assembly: AssemblyProduct("validator")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

public interface IValidatorLogger
{
  void LogErr(string format, params object[] arg);
  void LogStd(string format, params object[] arg);
}

public class ConsoleLogger : IValidatorLogger
{
  public void LogErr(string format, params object[] arg)
  {
    var lastColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine(format, arg);
    Console.ForegroundColor = lastColor;
  }
  
  public void LogStd(string format, params object[] arg)
  {
    Console.WriteLine(format, arg); 
  }
}

public class CheckSolutionForMissingProjectsLogic
{
  private Regex re = new Regex(@"<ProjectReference.*?<Project>(?'guid'.*?)</Project>.*?<Name>(?'name'.*?)</Name>.*?</ProjectReference>",
                               RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
  private readonly IValidatorLogger _logger;
  
  public CheckSolutionForMissingProjectsLogic(IValidatorLogger logger)
  {
    _logger = logger;    
  }
  
  private static bool IsCsharpProject(ProjectInSolution project)
  {
    return project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat;
  }

  bool AreAllReferencedProjectsPresent(SolutionFile solution)
  {
    foreach(var project in solution.ProjectsInOrder.Where(IsCsharpProject))
    {
      if (!File.Exists(project.AbsolutePath))
      {
        _logger.LogErr("Error SV001: Missing file {0}", project.AbsolutePath);
        return false;
      }
    }
    return true;
  }
  
  IEnumerable<String> GetAllProjects(SolutionFile solution)
  {
    return solution.ProjectsByGuid.Keys.Cast<String>();
  }
  
  string GetNameOfMissing(SolutionFile solution, String guid)
  {
    Regex re_name = new Regex(@"<ProjectReference.*?<Project>"+guid+"</Project>.*?<Name>(?'name'.*?)</Name>.*?</ProjectReference>",
                              RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
                            
    foreach(var project in solution.ProjectsInOrder.Where(IsCsharpProject))
      foreach (Match m in re_name.Matches(File.ReadAllText(project.AbsolutePath)))
        return m.Groups["name"].Value;
    return "unresolved name";
  }
  
  public bool Check(string SolutionPath)
  {
    var nothingIsMissing = true;

    _logger.LogStd("> {0}", SolutionPath);
    var solution = SolutionFile.Parse(SolutionPath);
    
    if (!AreAllReferencedProjectsPresent(solution))
      return false;
  
    var present = GetAllProjects(solution);
    
    foreach(var project in solution.ProjectsInOrder.Where(IsCsharpProject))
      foreach (Match m in re.Matches(File.ReadAllText(project.AbsolutePath)))
      {
        var needed = m.Groups["guid"].Value.ToUpper();
            
        if (!present.Contains(needed))
        {
          _logger.LogErr("{0}: Error SV002: Solution includes project '{1}' that is referencing project not included in that solution\n {2} = '{3}'\n '{1}' path is '{4}'", 
                         SolutionPath,
                         Path.GetFileNameWithoutExtension(project.AbsolutePath),
                         needed, GetNameOfMissing(solution, needed),
                         project.AbsolutePath); 
          nothingIsMissing = false;
        }
      }
      
    if (nothingIsMissing)
      _logger.LogStd("OK!");
    
    return nothingIsMissing;
  }
}

public static class Program
{
  static readonly int ErrorWrongUsage = 2;
  static readonly int ErrorFailedValidation = 1;
  static readonly int NoError = 0;
  
  public static int Main(string []args)
  {
    Console.ForegroundColor = ConsoleColor.White;
    var directory = Directory.GetCurrentDirectory();

    if (args.Length > 1)
    {
      Console.WriteLine("Error: This application accepts single parameter: Root path of the C# code repository");
      return ErrorWrongUsage;
    }
    else if (args.Length == 1)
    {
      directory = args.First();
    }
    
    Console.WriteLine("Checking for missing project references in solutions. ");
    
    var validationLogic = new CheckSolutionForMissingProjectsLogic(new ConsoleLogger());
    var allSolutions    = Directory.EnumerateFiles(directory, "*.sln", SearchOption.AllDirectories);
    var result          = true;
    
    try
    {
      foreach (var file in allSolutions)
        result = result & validationLogic.Check(file);
    }
    catch (Exception e)
    {
      Console.WriteLine("Fatal Error: {0}\n {1}", e.Message, e.StackTrace);
      result = false;
    }
   
    return result ? NoError : ErrorFailedValidation;
  }
}

