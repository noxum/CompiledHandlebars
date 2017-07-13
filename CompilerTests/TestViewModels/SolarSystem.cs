using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.TestViewModels
{

	public abstract class CelestialBodyModel
	{
		public string Description { get; set; }
		public string Name { get; set; }
		public double Mass { get; set; }
		public LinkModel WikiLink { get; set; }
	}

	public class MoonModel : CelestialBodyModel
	{
		public bool WasVisited { get; set; }
	}

	public class PlanetModel : CelestialBodyModel
	{
		public bool IsInhabited { get; set; }
		public bool IsHabitable { get; set; }
		public List<MoonModel> Moons { get; set; }
	}

	public class StarModel : CelestialBodyModel
	{
		public List<PlanetModel> Planets { get; set; }
	}


	public static class CelestialBodyFactory
	{
		public static StarModel CreateSolarSystem()
		{
			return new StarModel()
			{
				Planets = new List<PlanetModel>()
		  {
			 new PlanetModel()
			 {
				Name = "Mercury",
				Description = "Mercury is the smallest and closest to the Sun of the eight planets in the Solar System,[a] with an orbital period of about 88 Earth days. Seen from Earth, it appears to move around its orbit in about 116 days, which is much faster than any other planet in the Solar System. It has no known natural satellites. The planet is named after the Roman deity Mercury, the messenger to the gods.",
				IsHabitable = false,
				IsInhabited = false,
				Mass = 3.3e23,
				WikiLink = new LinkModel() {Text = "Mercury (planet)", Url = "https://en.wikipedia.org/wiki/Mercury_%28planet%29" }
			 },
			 new PlanetModel()
			 {
				Name = "Venus",
			 },
			 new PlanetModel()
			 {
				Name = "Earth",
				Moons = new List<MoonModel>()
				{
				  new MoonModel()
				  {
					 Name = "Moon"
				  }
				}
			 },
			 new PlanetModel()
			 {
				Name = "Mars",
				IsHabitable = false,
				IsInhabited = false,
				Mass = 3.3e23,
				Moons = new List<MoonModel>()
				{
				  new MoonModel()
				  {
					 Name = "Deimos"
				  },
				  new MoonModel()
				  {
					 Name = "Phobos"
				  }
				}
			 }
		  },
				Name = "Sun",
				Description = "The Sun[a] is the star at the center of the Solar System and is by far the most important source of energy for life on Earth. It is a nearly perfect spherical ball of hot plasma,[12][13] with internal convective motion that generates a magnetic field via a dynamo process.[14] Its diameter is about 109 times that of Earth, and it has a mass about 330,000 times that of Earth, accounting for about 99.86% of the total mass of the Solar System.[15] About three quarters of the Sun's mass consists of hydrogen; the rest is mostly helium, with much smaller quantities of heavier elements, including oxygen, carbon, neon and iron.[16]",
				Mass = 2e30,
				WikiLink = new LinkModel() { Text = "Sun", Url = "https://en.wikipedia.org/wiki/Sun" }
			};
		}
	}
}
