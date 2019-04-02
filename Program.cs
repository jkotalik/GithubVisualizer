using Octokit;
using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp9
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var owner = "aspnet";
            var repoName = "aspnetcore";

            // Pull requests
            // average time for PR open
            // Average time for issue open
            // Average time for issue triaged
            // average time for PR from open reviewed
            // How to cache stuff

            // How to make this extensible

            // calculate averages each time, but don't 
            // Things to enumerate
            // repos
            // issues
            // pull requests
            // checks per commit
            using (var db = new CheckContext())
            {
                foreach (var check in db.CheckTypes.Include(b => b.Checks).ToList())
                {
                    Console.WriteLine(" - {0}", check.Name);
                }
                try
                {
                    var client = new GitHubClient(new ProductHeaderValue("aspnetcore"));
                    client.Credentials = new Credentials(token);
                    //client.Credentials = new Credentials(token);
                    //var pullRequests = await client.PullRequest.GetAllForRepository(owner, repoName);
                    //var res = await client.Repository.GetAllForCurrent();
                    var pullRequests = await client.PullRequest.GetAllForRepository(owner, repoName);
                    // should be able to updated - created?
                    // after getting the checks, how do I get info from it.
                    foreach (var pr in pullRequests)
                    {
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

                            CheckType checkModel = await db.CheckTypes.FirstOrDefaultAsync(c => c.Name == check.Name);
                            if (checkModel == null)
                            {
                                var ck = new Check { PullRequestName = check.Name, TimeTaken = finished.Value - start};
                                checkModel = new CheckType { Name = check.Name, Checks = new List<Check>{ ck } };
                                db.CheckTypes.Add(checkModel);
                            }
                            else
                            {
                                var c2 = await db.Checks.FirstOrDefaultAsync(c => c.PullRequestName == pr.Url && c.CheckType.Name == check.Name);
                                if (c2 == null)
                                {
                                    var ck = new Check { PullRequestName = pr.Url, TimeTaken = finished.Value - start};
                                    checkModel.Checks.Add(ck);
                                }
                            }
                            var count = db.SaveChanges();
              
                            Console.WriteLine("{0} records saved to database", count);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                foreach (var l in db.CheckTypes)
                {
                    Console.WriteLine(l.Name);
                    var total = TimeSpan.Zero;
                    var count = 0;
                    foreach (var i in l.Checks)
                    {
                        total += i.TimeTaken;
                        count++;
                    }

                    Console.WriteLine($"Average time {total / count}");
                }
            }
        }
    }
}
