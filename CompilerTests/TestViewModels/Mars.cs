using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.TestViewModels
{
  public class MarsModel : PlanetModel
  {
    public MoonModel Phobos { get; set; }
    public MoonModel Deimos { get; set; }
  }

  public static class MarsModelFactory
  {
    public static MarsModel CreateFullMarsModel()
    {
      var model = new MarsModel();
      model.Name = "Mars";
      model.HasMoons = true;
      model.Phobos = new MoonModel() { Name = "Phobos" };
      model.Deimos = new MoonModel() { Name = "Deimos" };
      model.WikiLink = new LinkModel()
      {
        Url = "https://en.wikipedia.org/wiki/Mars",
        Text = "Mars"
      };
      model.Description = "<b>Mars</b> is the fourth <a href=\"/wiki/Planet\" title=\"Planet\">planet</a> from the <a href=\"/wiki/Sun\" title=\"Sun\">Sun</a> and the second smallest planet in the <a href=\"/wiki/Solar_System\" title=\"Solar System\">Solar System</a>, after <a href=\"/wiki/Mercury_(planet)\" title=\"Mercury (planet)\">Mercury</a>.";
      return model;
    }
  }
}
