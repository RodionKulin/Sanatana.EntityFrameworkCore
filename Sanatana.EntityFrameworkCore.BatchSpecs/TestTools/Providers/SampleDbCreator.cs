﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Sanatana.EntityFrameworkCore.BatchSpecs.Samples;
using Sanatana.EntityFrameworkCore.BatchSpecs.TestTools.Interfaces;
using SpecsFor.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.EntityFrameworkCore.BatchSpecs.TestTools.Providers
{
    public class SampleDbCreator : Behavior<INeedSampleDatabase>
    {
        //fields
        private static bool _isInitialized;


        //methods
        public override void SpecInit(INeedSampleDatabase instance)
        {
            if (_isInitialized)
            {
                return;
            }

            instance.SampleDatabase.Database.EnsureDeleted();
            instance.SampleDatabase.Database.EnsureCreated();
          

            _isInitialized = true;
        }
    }
}
