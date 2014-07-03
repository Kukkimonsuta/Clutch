using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Clutch.Web.Mvc.Routing
{
	/// <summary>
	/// Class serving as root for fluent routing
	/// </summary>
	public class FluentRouter
	{
		public FluentRouter(RouteCollection routes, string areaName = null)
		{
			if (routes == null)
				throw new ArgumentNullException("routes");

			m_routes = routes;
			m_areaName = areaName;
		}

		private RouteCollection m_routes;
		private string m_areaName;
		private Action<IFluentRouteConfiguration> m_template;

		private Route Create(string name, string url, FluentRouteConfiguration config)
		{
			var route = default(Route);

			var handler = config.Handler ?? new MvcRouteHandler();
			var defaults = config.Defaults == null ? new RouteValueDictionary() : new RouteValueDictionary(config.Defaults);
			var constraints = config.Constraints == null ? new RouteValueDictionary() : new RouteValueDictionary(config.Constraints);
			var dataTokens = config.DataTokens == null ? new RouteValueDictionary() : new RouteValueDictionary(config.DataTokens);

			// create complex route only when required
			if (config.Rules != null && config.Rules.Any())
				// no need to clone rules, it's always cloned from within MvcComplexRoute
				route = new MvcComplexRoute(url, config.Rules, handler);
			else
				route = new Route(url, handler);

			route.Defaults = defaults;
			route.Constraints = constraints;
			route.DataTokens = dataTokens;

			if ((config.Namespaces != null) && (config.Namespaces.Length > 0))
			{
				route.DataTokens["Namespaces"] = config.Namespaces;
			}

			if (m_areaName != null && m_areaName.Length > 0)
			{
				route.DataTokens["area"] = m_areaName;
				route.DataTokens["UseNamespaceFallback"] = config.Namespaces == null || config.Namespaces.Length == 0;
			}

			return route;
		}

		/// <summary>
		/// Set default route configuration
		/// </summary>
		public FluentRouter Template(Action<IFluentRouteConfiguration> configuration)
		{
			m_template = configuration;

			return this;
		}

		/// <summary>
		/// Maps new route according to configuration
		/// </summary>
		public FluentRouter Map(string name, Action<IFluentRouteConfiguration> configuration)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (configuration == null)
				throw new ArgumentNullException("configuration");

			var config = new FluentRouteConfiguration();

			if (m_template != null)
				m_template(config);

			configuration(config);

			// create new route for each url; first url is considered to be default
			var urls = config.Urls.Reverse().ToArray();

			for (var i = 0; i < urls.Length; i++)
			{
				var pair = urls[i];

				var isDefault = i + 1 == urls.Length;
				var culture = pair.Key;
				var routeName = isDefault ? name : name + ":" + culture;

				var route = Create(routeName, pair.Value, config);

				route.Defaults.Add("culture", culture);

				// default route doesn't take culture into consideration
				if (!isDefault)
					route.Constraints.Add("_culture", new CultureConstraint(culture));

				m_routes.Add(routeName, route);
			}

			return this;
		}
	}

	public interface IFluentRouteConfiguration
	{
		IFluentRouteConfiguration Url(string culture, string url);

		IFluentRouteConfiguration Defaults(object defaults);
		IFluentRouteConfiguration Defaults(RouteValueDictionary defaults);

		IFluentRouteConfiguration Constraints(object constraints);
		IFluentRouteConfiguration Constraints(RouteValueDictionary constraints);

		IFluentRouteConfiguration DataTokens(object dataTokens);
		IFluentRouteConfiguration DataTokens(RouteValueDictionary dataTokens);

		IFluentRouteConfiguration Rules(object rules);
		IFluentRouteConfiguration Rules(RouteValueDictionary rules);

		IFluentRouteConfiguration Namespaces(string[] namespaces);

		IFluentRouteConfiguration Handler(IRouteHandler handler);
	}

	public class FluentRouteConfiguration : IFluentRouteConfiguration
	{
		public FluentRouteConfiguration()
		{
			Urls = new Dictionary<string, string>();
		}

		public IDictionary<string, string> Urls { get; set; }

		public RouteValueDictionary Defaults { get; set; }
		public RouteValueDictionary Constraints { get; set; }
		public RouteValueDictionary Rules { get; set; }
		public RouteValueDictionary DataTokens { get; set; }

		public string[] Namespaces { get; set; }
		public IRouteHandler Handler { get; set; }

		#region IFluentRouteConfiguration

		IFluentRouteConfiguration IFluentRouteConfiguration.Url(string culture, string url)
		{
			if (culture == null)
				throw new ArgumentNullException("culture");
			if (url == null)
				throw new ArgumentNullException("url");

			Urls.Add(culture, url);

			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.Defaults(object defaults)
		{
			Defaults = new RouteValueDictionary(defaults);
			return this;
		}
		IFluentRouteConfiguration IFluentRouteConfiguration.Defaults(RouteValueDictionary defaults)
		{
			Defaults = defaults;
			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.Constraints(object constraints)
		{
			Constraints = new RouteValueDictionary(constraints);
			return this;
		}
		IFluentRouteConfiguration IFluentRouteConfiguration.Constraints(RouteValueDictionary constraints)
		{
			Constraints = constraints;
			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.DataTokens(object dataTokens)
		{
			DataTokens = new RouteValueDictionary(dataTokens);
			return this;
		}
		IFluentRouteConfiguration IFluentRouteConfiguration.DataTokens(RouteValueDictionary dataTokens)
		{
			DataTokens = dataTokens;
			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.Rules(object rules)
		{
			Rules = new RouteValueDictionary(rules);
			return this;
		}
		IFluentRouteConfiguration IFluentRouteConfiguration.Rules(RouteValueDictionary rules)
		{
			Rules = rules;
			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.Namespaces(string[] namespaces)
		{
			Namespaces = namespaces;
			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.Handler(IRouteHandler handler)
		{
			Handler = handler;
			return this;
		}

		#endregion
	}
}
