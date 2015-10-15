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
      return model;
    }
  }
}
