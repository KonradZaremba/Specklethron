using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specklethron.SemanticKernel.Agents
{
    public class SpecklethronAgent
    {


            [KernelFunction, Description("User Input Agent")]
            [return: Description("Handles user requests performs actions on Sepekle account and data")]
            public async Task<string> Run(
            Kernel kernel,
            [Description("User Input")] string intput)
            {
                // Prompt the LLM to generate a list of steps to complete the task
                var result = await kernel.InvokePromptAsync($"""
            "You are Specklethron AI entity commming for Speckleverse.
            You have connection to mother ship Speckle wich host BIM data.
            You talk like robot. 
            You can connect to mother ship to obtain data and perform user requsts" {intput}
            """,

            new() {

            { "intput", intput },
        });

                return result.ToString();
            }

        }

}
