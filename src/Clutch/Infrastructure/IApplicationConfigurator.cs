using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Clutch.Infrastructure
{
    /// <summary>
    /// Provides access to application configuration
    /// </summary>
    public interface IApplicationConfigurator
    {
        /// <summary>
        /// Returns application assembly
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// Returns whether current application runs in debug mode
        /// </summary>
        bool IsDebug { get; }

        /// <summary>
        /// Returns whether current application runs in test mode
        /// </summary>
        bool IsTest { get; }
    }
}
