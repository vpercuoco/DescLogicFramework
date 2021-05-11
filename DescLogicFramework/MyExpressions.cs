using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DescLogicFramework
{
    public class MyExpressions
    {

        public void CreateExpression()
        {

            //Exploring a premade lamba:
            Expression<Func<int,bool>> IsGreaterThanFive = (int x) => x > 5;
            Expression body = IsGreaterThanFive.Body;

            var t = ExpressionType.Constant;
            
          
            
            ParameterExpression numerator = Expression.Parameter(typeof(int));
            ParameterExpression denominator = Expression.Parameter(typeof(int));

           //TryExpression myTry = Expression.TryCatchFinally()

            

            var divide = ExpressionType.Divide;


           BinaryExpression combination = Expression.MakeBinary(divide, numerator, denominator);
           Expression<Func<int, int, int>> division = Expression.Lambda<Func<int, int, int>>(combination);

            Func<int, int, int> compiledDivision = division.Compile();

            var result = compiledDivision.Invoke(5, 5);

            Console.WriteLine(result.ToString());
       

        }
        

    }
}
