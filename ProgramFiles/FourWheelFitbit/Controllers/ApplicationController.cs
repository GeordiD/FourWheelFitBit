﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using FourWheelFitbit.AlgorithmAnalysis;
using FourWheelFitbit.Models;
using System.Data;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FourWheelFitbit.Controllers
{
    [Route("api/[controller]")]
    public class ApplicationController : Controller
    {
        [Route("~/api/test")]
        [HttpGet]
        public string TestConnection() {
            return "Connected to FourWheelFitBit: " + DateTime.Now.ToString();
        }

        // POST api/<controller>
        // This is the api for the Android App to upload the recorded wheelchair data to. Data will be a CSV string in the following format:
        // x-axis, y-axis, z-axis, timestamp;
        [HttpPost]
        public IActionResult Post(string inputData)
        {
            try
            {
                DataTable inputDataTable = new WheelchairData(inputData).wheelchairDataTable;
                WheelchairMovementAnalyzer algo = new WheelchairMovementAnalyzer();
                List<ResultSet> result = algo.AnalyzeDataTable(inputDataTable);
                Int64 moveTime = algo.MoveTime;
                Int64 stillTime = algo.StillTime;
                return Ok(new { results = result, moveTimeTotal = moveTime, stillTimeTotal = stillTime });
            }
            catch
            {
                return BadRequest("There was a problem analyzing your input. Please try again.");
            }
        }

        [HttpPost("upload")]
        public IActionResult Post(IFormFile file)
        {
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            // verify .csv file extension
            if (!fileName.ToLower().EndsWith(".csv")){
                return BadRequest("There was a problem with your file extension. Please make sure that it is properly formatted a .csv file.");
            }

            try
            {
                string inputData;

                using (StreamReader stream = new StreamReader(file.OpenReadStream()))
                {
                    inputData = stream.ReadToEnd();
                }

                DataTable inputDataTable = new WheelchairData(inputData).wheelchairDataTable;
                // Run analyzer here and return the text to the view
                WheelchairMovementAnalyzer algo = new WheelchairMovementAnalyzer();
                List<ResultSet> result = algo.AnalyzeDataTable(inputDataTable);
                Int64 moveTime = algo.MoveTime;
                Int64 stillTime = algo.StillTime;
                return Ok(new { results = result, moveTimeTotal = moveTime, stillTimeTotal = stillTime });
            }
            catch
            {
                return StatusCode(500, "There was a problem analyzing your file. Please make sure it is properly formatted and try again.");
            }
        }
    }
}
