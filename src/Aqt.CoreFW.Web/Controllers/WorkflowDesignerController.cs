using Aqt.CoreFW.Web.Workflows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OptimaJet.Workflow;
using OptimaJet.Workflow.Core.Runtime;
using OptimaJet.Workflow.Oracle;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System;
using System.Threading.Tasks;

namespace Aqt.CoreFW.Web.Controllers;

public class WorkflowDesignerController : Controller
{
    private readonly WorkflowRuntime _runtime;

    public WorkflowDesignerController(WorkflowRuntime runtime)
    {
        _runtime = runtime;
    }

    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> Api()
    {
        Stream? filestream = null;
        var parameters = new NameValueCollection();

        //Defining the request method
        var isPost = Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase);

        //Parse the parameters in the query string
        foreach (var q in Request.Query)
        {
            parameters.Add(q.Key, q.Value.First());
        }

        if (isPost)
        {
            //Parsing the parameters passed in the form
            var keys = parameters.AllKeys;

            foreach (var key in Request.Form.Keys)
            {
                if (!keys.Contains(key))
                {
                    parameters.Add(key, Request.Form[key]);
                }
            }

            //If a file is passed
            if (Request.Form.Files.Count > 0)
            {
                //Save file
                filestream = Request.Form.Files[0].OpenReadStream();
            }
        }

        //Calling the Designer Api and store answer
        var (result, hasError) = await _runtime.DesignerAPIAsync(parameters, filestream);

        //If it returns a file, send the response in a special way
        if (parameters["operation"]?.ToLower() == "downloadscheme" && !hasError)
            return File(Encoding.UTF8.GetBytes(result), "text/xml");

        //response
        return Content(result);
    }

    /// <summary>
    /// GetAllActivities
    /// </summary>
    public async Task<IActionResult> GetAllActivities()
    {
        var activities = new string[] { "Created", "PendingApproval", "Approved" };
        return Json(activities);
    }

    /// <summary>
    /// GetAllStates
    /// </summary>
    public async Task<IActionResult> GetAllStates()
    {
        var states = new string[] { "Created", "PendingApproval", "Approved" };
        return Json(states);
    }
}