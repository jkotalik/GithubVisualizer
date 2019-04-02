using Octokit;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp9
{
    class Program
    {
        static int Main(string[] args)
        {
            var application = new CommandLineApplication();
            application.HelpOption();
            var tokenOption = application.Option("-gat|--github-access-token <ACCESSTOKEN>", 
                "The github personal access token required to query the repository", 
                CommandOptionType.SingleValue);
            // Add option for connection string
            application.OnExecute(async () =>
            {
                var token = tokenOption.Value();
                await PrintGithubStatistics();
            });
            return application.Execute();
        }
        
        // Three steps for flow
        // 1. Acquire and update data from github
        // 2. calculate important stats from data
        // 3. Send data to be visualized.

        private static async Task PrintGithubStatistics()
        {
            var owner = "aspnet";
            var repoName = "aspnetcore";

            // Pull requests
            // average time for PR open
            // Average time for issue open
            // Average time for issue triaged
            // average time for PR from open reviewed
            // How to cache stuff

            // calculate averages each time, but don't 
            // Things to enumerate
            // repos
            // issues
            // pull requests
            // checks per commit
            var configBuilder = new ConfigurationBuilder()
              .AddUserSecrets<AdminInformation>();
            var root = configBuilder.Build();
            var token = root["token"];

            using (var db = new CheckContext())
            {
                try
                {
                    var client = new GitHubClient(new ProductHeaderValue("aspnetcore"));
                    client.Credentials = new Credentials(token);
                    //client.Credentials = new Credentials(token);
                    //var pullRequests = await client.PullRequest.GetAllForRepository(owner, repoName);
                    //var res = await client.Repository.GetAllForCurrent();
                    var prRequest = new PullRequestRequest();
                    prRequest.State = ItemStateFilter.All;
                    prRequest.Base = "master";
                    prRequest.SortDirection = SortDirection.Descending;
                    var pullRequests = await client.PullRequest.GetAllForRepository(owner, repoName, prRequest);
                    // Get all closed pull requests too.
                    // should be able to updated - created?
                    // after getting the checks, how do I get info from it.
                    for (var i = 0; i < 100; i++)
                    {
                        var pr = pullRequests[i];
                        // check if pr is pointing to master
                        var checks = await client.Check.Run.GetAllForReference(owner, repoName, pr.Head.Sha);
                        foreach (var check in checks.CheckRuns)
                        {
                            // Primary key name is the check name
                            // value is average time?
                            // actually the db may need to be ever expanding 
                            var start = check.StartedAt;
                            var finished = check.CompletedAt;
                            var status = check.Conclusion;
                            if (status != CheckConclusion.Success)
                            {
                                continue;
                            }

                            CheckType checkModel = await db.CheckTypes.Include(c => c.Checks).FirstOrDefaultAsync(c => c.Name == check.Name);
                            if (checkModel == null)
                            {
                                var ck = new Check { PullRequestName = pr.Url, SHA = pr.Head.Sha, TimeTaken = (finished.Value - start).TotalMinutes, Start = start, Finished = finished.Value};
                                checkModel = new CheckType { Name = check.Name, Checks = new List<Check>{ ck } };
                                db.CheckTypes.Add(checkModel);
                            }
                            else
                            {
                                var check2 = await db.Checks.FirstOrDefaultAsync(c => c.PullRequestName == pr.Url && c.CheckType.Name == check.Name);
                                if (check2 == null)
                                {
                                    var ck = new Check { PullRequestName = pr.Url, SHA = pr.Head.Sha, TimeTaken = (finished.Value - start).TotalMinutes, Start = start, Finished = finished.Value };
                                    checkModel.Checks.Add(ck);
                                }
                            }
                            var count = db.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        // Timestamps
        // per commit
        // Just track aspnetcore-ci
        // port to 3.0
    }
}
