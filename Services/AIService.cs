using Microsoft.CodeAnalysis;
using Npgsql;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkMonitor_360.Models;


namespace WorkMonitor_360.Services
{
    public class AIService
    {
        private readonly string _apiKey;
        private readonly ChatClient _chatClient;

        public AIService() 
        {

            //_apiKey = ConfigurationManager.AppSettings["OpenAIApiKey"];
            _apiKey = ConfigurationManager.AppSettings["OpenAIApiKey"]
             ?? throw new InvalidOperationException(
                 "OpenAIApiKey is missing from App.config.");
            _chatClient = new ChatClient(model: "gpt-4.1-mini", apiKey: _apiKey);

        }

        public async Task<string> GenerateReportAsync(    
        string machineName ,
        string userName,
        decimal productiveTime,
        decimal idleTime)   
        {
            try
            {

                string prompt = $@"
                Analyze employee activity.
            
                Machine Name : {machineName}
                User Name : {userName}
                Productive Hours : {productiveTime}
                Idle Hours : {idleTime}            

                Generate:
                1. Summary
                2. Productivity score
                3. Recommendations";


                //After configuring the Paid Licensed Web API key we can un comment the below 3 lines and use it.
                ChatCompletion completion =
                   await _chatClient.CompleteChatAsync(prompt);
                return completion.Content[0].Text;


                //Temporarily hardcode due to free version iam using
                //string machine = machineName;
                //string employee = userName;               
                //decimal productiveHours = productiveTime;
                //decimal idleHours = idleTime;

                //decimal totalHours = productiveHours + idleHours;
                //decimal productivityPercentage = (productiveHours / totalHours) * 100;
                //decimal idlePercentage = (idleHours / totalHours) * 100;

                //  string  reportResponseData = $@"
                //        AI Employee Activity Report

                //        Employee: {employee}
                //        Machine Name: {machine}

                //        1. Summary

                //        The employee recorded {productiveHours:0.##} productive hours ({productiveHours * 60:0} minutes) and {idleHours:0.##} idle hours ({idleHours * 60:0} minutes) during the analyzed period. This indicates that the system was idle for the vast majority of the tracked time, resulting in very low productive activity.

                //        Key Metrics

                //        • Productive Time : {productiveHours:0.##} hours ({productiveHours * 60:0} minutes)
                //        • Idle Time       : {idleHours:0.##} hours ({idleHours * 60:0} minutes)
                //        • Total Time      : {totalHours:0.##} hours
                //        • Productivity    : {productivityPercentage:0.00}%
                //        • Idle            : {idlePercentage:0.00}%
                //        ";

                //return reportResponseData;

                //Temporarily hardcode due to free version iam using

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Unable to generate AI report");
                return "Unable to generate AI report.";

            }
        }      

    }
}
