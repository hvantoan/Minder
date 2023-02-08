namespace Minder.Service.Jobs.Config {

    public class JobConfig {
        public string Name { get; set; } = null!;
        public int BatchWrite { get; set; } = 1000;
    }
}