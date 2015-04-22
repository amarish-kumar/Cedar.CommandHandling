﻿namespace Cedar.CommandHandling.Http
{
    using System;

    public class HttpProblemDetailsException<T> : Exception, IHttpProblemDetailException
        where T : HttpProblemDetails
    {
        private readonly T _problemDetails;

        public HttpProblemDetailsException(T problemDetails)
            : base(problemDetails.Title)
        {
            if (problemDetails == null)
            {
                throw new ArgumentNullException("problemDetails");
            }
            _problemDetails = problemDetails;
        }

        public T ProblemDetails
        {
            get { return _problemDetails; }
        }

        HttpProblemDetails IHttpProblemDetailException.ProblemDetails
        {
            get { return _problemDetails; }
        }
    }
}