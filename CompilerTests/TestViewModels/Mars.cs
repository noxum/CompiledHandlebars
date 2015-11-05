using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.TestViewModels
{
  public class MarsModel : PlanetModel
  {
    public int MoonCount { get; set; }
    public MoonModel Phobos { get; set; }
    public MoonModel Deimos { get; set; }
    public Planitia[] Plains { get; set; }
    public List<Mountain> Mountains { get; set; }
    public Dictionary<string, Rover> Rovers { get; set; }
    public class Planitia
    {
      public string Name { get; set; }
    }

    public class Mountain
    {
      public string Name { get; set; }
    }

    public class Rover
    {
      public string Name { get; set; }
    }
  }

  public static class MarsModelFactory
  {
    public static MarsModel CreateFullMarsModel()
    {
      var model = new MarsModel();
      model.Name = "Mars";
      model.Phobos = new MoonModel() { Name = "Phobos" };
      model.Deimos = new MoonModel() { Name = "Deimos" };
      model.MoonCount = 2;
      model.WikiLink = new LinkModel()
      {
        Url = "https://en.wikipedia.org/wiki/Mars",
        Text = "Mars"
      };
      model.Plains = new MarsModel.Planitia[] {
        new MarsModel.Planitia() { Name = "Acidalia Planitia"},
        new MarsModel.Planitia() { Name = "Utopia Planitia"},
      };
      model.Mountains = new List<MarsModel.Mountain>()
      {
        new MarsModel.Mountain() { Name = "Aeolis Mons"},
        new MarsModel.Mountain() { Name = "Olympus Mons"},
      };
      model.Rovers = new Dictionary<string, MarsModel.Rover>()
      {
        { "Opportunity", new MarsModel.Rover() {Name = "Opportunity" } },
        { "Curiosity", new MarsModel.Rover() {Name = "Curiosity" } },
      };
      model.Description = "<b>Mars</b> is the fourth <a href=\"/wiki/Planet\" title=\"Planet\">planet</a> from the <a href=\"/wiki/Sun\" title=\"Sun\">Sun</a> and the second smallest planet in the <a href=\"/wiki/Solar_System\" title=\"Solar System\">Solar System</a>, after <a href=\"/wiki/Mercury_(planet)\" title=\"Mercury (planet)\">Mercury</a>.";
      return model;
    }
  }
}
