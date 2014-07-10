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

			m_scopes = new Stack<Action<IFluentRouteConfiguration>>();
		}

		private RouteCollection m_routes;
		private string m_areaName;
		private Action<IFluentRouteConfiguration> m_template;
		private Stack<Action<IFluentRouteConfiguration>> m_scopes;

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
		/// Allows use of new template after the old one.
		/// </summary>
		public FluentRouter Scope(Action<FluentRouter> scope)
		{
			m_scopes.Push(m_template);
			try
			{
				scope(this);
			}
			finally
			{
				m_template = m_scopes.Pop();
			}

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

			foreach (var template in m_scopes.Where(s => s != null))
				template(config);

			if (m_template != null)
				m_template(config);

			configuration(config);

			// create new route for each url; first url is considered to be default
			var urls = config.Urls.Reverse().ToArray();

			if (!string.IsNullOrEmpty(config.NamePrefix))
				name = config.NamePrefix + "_" + name;

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
		IFluentRouteConfiguration NamePrefix(string prefix, bool append = true);

		IFluentRouteConfiguration Url(string culture, string url);

		IFluentRouteConfiguration Defaults(object defaults, bool merge = true);
		IFluentRouteConfiguration Defaults(RouteValueDictionary defaults, bool merge = true);

		IFluentRouteConfiguration Constraints(object constraints, bool merge = true);
		IFluentRouteConfiguration Constraints(RouteValueDictionary constraints, bool merge = true);

		IFluentRouteConfiguration DataTokens(object dataTokens, bool merge = true);
		IFluentRouteConfiguration DataTokens(RouteValueDictionary dataTokens, bool merge = true);

		IFluentRouteConfiguration Rules(object rules, bool merge = true);
		IFluentRouteConfiguration Rules(RouteValueDictionary rules, bool merge = true);

		IFluentRouteConfiguration Namespaces(string[] namespaces, bool merge = true);

		IFluentRouteConfiguration Handler(IRouteHandler handler);
	}

	public class FluentRouteConfiguration : IFluentRouteConfiguration
	{
		public FluentRouteConfiguration()
		{
			Urls = new Dictionary<string, string>();
		}

		public string NamePrefix { get; set; }

		public IDictionary<string, string> Urls { get; set; }

		public RouteValueDictionary Defaults { get; set; }
		public RouteValueDictionary Constraints { get; set; }
		public RouteValueDictionary Rules { get; set; }
		public RouteValueDictionary DataTokens { get; set; }

		public string[] Namespaces { get; set; }
		public IRouteHandler Handler { get; set; }

		#region IFluentRouteConfiguration

		IFluentRouteConfiguration IFluentRouteConfiguration.NamePrefix(string prefix, bool append)
		{
			if (NamePrefix == null || !append)
				NamePrefix = prefix;
			else if (prefix != null)
				NamePrefix += "_" + prefix;

			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.Url(string culture, string url)
		{
			if (culture == null)
				throw new ArgumentNullException("culture");
			if (url == null)
				throw new ArgumentNullException("url");

			Urls.Add(culture, url);

			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.Defaults(object defaults, bool merge)
		{
			((IFluentRouteConfiguration)this).Defaults(new RouteValueDictionary(defaults), merge: merge);
			return this;
		}
		IFluentRouteConfiguration IFluentRouteConfiguration.Defaults(RouteValueDictionary defaults, bool merge)
		{
			if (Defaults == null || !merge)
				Defaults = defaults;
			else
				foreach (var pair in defaults)
					Defaults[pair.Key] = pair.Value;

			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.Constraints(object constraints, bool merge)
		{
			((IFluentRouteConfiguration)this).Constraints(new RouteValueDictionary(constraints), merge: merge);
			return this;
		}
		IFluentRouteConfiguration IFluentRouteConfiguration.Constraints(RouteValueDictionary constraints, bool merge)
		{
			if (Constraints == null || !merge)
				Constraints = constraints;
			else
				foreach (var pair in constraints)
					Constraints[pair.Key] = pair.Value;

			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.DataTokens(object dataTokens, bool merge)
		{
			((IFluentRouteConfiguration)this).DataTokens(new RouteValueDictionary(dataTokens), merge: merge);
			return this;
		}
		IFluentRouteConfiguration IFluentRouteConfiguration.DataTokens(RouteValueDictionary dataTokens, bool merge)
		{
			if (DataTokens == null || !merge)
				DataTokens = dataTokens;
			else
				foreach (var pair in dataTokens)
					DataTokens[pair.Key] = pair.Value;

			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.Rules(object rules, bool merge)
		{
			((IFluentRouteConfiguration)this).Rules(new RouteValueDictionary(rules), merge: merge);
			return this;
		}
		IFluentRouteConfiguration IFluentRouteConfiguration.Rules(RouteValueDictionary rules, bool merge)
		{
			if (Rules == null || !merge)
				Rules = rules;
			else
				foreach (var pair in rules)
					Rules[pair.Key] = pair.Value;

			return this;
		}

		IFluentRouteConfiguration IFluentRouteConfiguration.Namespaces(string[] namespaces, bool merge)
		{
			if (Namespaces == null || !merge)
				Namespaces = namespaces;
			else
				Namespaces = Namespaces.Concat(namespaces).ToArray();

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
