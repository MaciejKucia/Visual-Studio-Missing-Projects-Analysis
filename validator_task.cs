//-----------------------------------------------------------------------
// <copyright file="validator.cs" company="Maciej Kucia">
//     MIT License https://github.com/MaciejKucia/Visual-Studio-Missing-Projects-Analysis
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Construction;
using System.Text.RegularExpressions;

public class MSBuildLogger : IValidatorLogger
{
  private readonly TaskLoggingHelper _logHelper;
  
  public MSBuildLogger(TaskLoggingHelper logHelper)
  {
    _logHelper = logHelper;
  }
  
  public void LogErr(string format, params object[] arg)
  {
    _logHelper.LogError(format, arg);
  }
  
  public void LogStd(string format, params object[] arg)
  {
    _logHelper.LogMessage(format, arg);
  }
}

public class CheckSolutionForMissingProjects : Task
{
  [Required]
  public ITaskItem[] SolutionPaths { get; set; }
    
  public override bool Execute()
  {
    var validationLogic = new CheckSolutionForMissingProjectsLogic(new MSBuildLogger(this.Log));
    var ret = true;
    
    foreach (var solutionPath in SolutionPaths.Select(i => i.ItemSpec))
      ret = ret & validationLogic.Check(solutionPath);

    return ret;
  }
}