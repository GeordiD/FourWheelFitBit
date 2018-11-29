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
        // POST api/<controller>
        // This is the api for the Android App to upload the recorded wheelchair data to. Data will be a CSV string in the following format:
        // x-axis, y-axis, z-axis, timestamp;
        [HttpPost]
        public DataTable Post(string inputData)
        {
            DataTable inputDataTable = new WheelchairData(inputData).wheelchairDataTable;
            // TODO this is not final code, this should all be cleaned up and the vars renamed
            WheelchairMovementAnalyzer algo = new WheelchairMovementAnalyzer();

            //TODO GED - I don't really know why there are two post methods. After changing the type
            //     from WheelchairMovementAnalyzer to ResultSet, it got angry and I muted it for later
            //return algo.AnalyzeDataTable(inputDataTable);
            return null;
        }

        [HttpPost("upload")]
        public IActionResult Post(IFormFile file)
        {
            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            //verify .csv file extension
            if (!fileName.EndsWith(".csv")){
                return BadRequest();
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
                // TODO this is nots final code, this should all be cleaned up and the vars renamed
                WheelchairMovementAnalyzer algo = new WheelchairMovementAnalyzer();
                List<ResultSet> result = algo.AnalyzeDataTable(inputDataTable);
                return Ok(new { results = result });
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
