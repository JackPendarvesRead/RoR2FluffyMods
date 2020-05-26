using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace TestStuff
{
    class EventStuff
    {
        ////void delegates with no parameters
        //static public Delegate Create(EventInfo evt, Action d)
        //{
        //    var handlerType = evt.EventHandlerType;
        //    var eventParams = handlerType.GetMethod("Invoke").GetParameters();

        //    //lambda: (object x0, EventArgs x1) => d()
        //    var parameters = eventParams.Select(p => Expression.Parameter(p.ParameterType, "x"));
        //    var body = Expression.Call(Expression.Constant(d), d.GetType().GetMethod("Invoke"));
        //    var lambda = Expression.Lambda(body, parameters.ToArray());
        //    return Delegate.CreateDelegate(handlerType, lambda.Compile(), "Invoke", false);
        //}

        //void delegate with one parameter
        internal static Delegate Create(EventInfo evt, Action<ILContext> d)
        {
            var handlerType = evt.EventHandlerType;
            var eventParams = handlerType.GetMethod("Invoke").GetParameters();

            //lambda: (object x0, ExampleEventArgs x1) => d(x1.IntArg)
            var parameters = eventParams.Select(p => Expression.Parameter(p.ParameterType, "il")).ToArray();
            var body = Expression.Call(Expression.Constant(d), d.GetType().GetMethod("Invoke"), parameters);
            var lambda = Expression.Lambda(body, parameters);
            return Delegate.CreateDelegate(handlerType, lambda.Compile(), "Invoke", false);
        }
    }
}
