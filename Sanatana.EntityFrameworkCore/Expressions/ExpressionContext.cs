﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.EntityFrameworkCore.Expressions
{
    public class ExpressionContext
    {
        //properties
        public List<string> Arguments { get; set; }
        public Expression ParentExpression { get; set; }
        public DbContext DbContext { get; set; }


        //init
        public ExpressionContext(Expression parentExpression, DbContext context)
        {
            ParentExpression = parentExpression;
            DbContext = context;
        }


        //methods
        public void SetArguments(LambdaExpression lambda)
        {
            if (Arguments != null)
            {
                throw new Exception("Lambda arguments already set.");
            }

            Arguments = lambda.Parameters
                .Select(p => p.Name)
                .ToList();
        }

        public ExpressionContext Copy()
        {
            return new ExpressionContext(ParentExpression, DbContext)
            {
                Arguments = Arguments
            };
        }

    }
}
