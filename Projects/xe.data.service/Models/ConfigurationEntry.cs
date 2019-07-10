namespace xe.data.service.Models
{
    public class ConfigurationEntry
    {
        public virtual string DatabaseType { get; set; }
        public virtual string ConnectionString { get; set; }
        public virtual string SqlCommand { get; set; }
        public virtual int CommandTimeout { get; set; }
        public virtual string Name { get; set; }
        public virtual string Parameters { get; set; }
    }
}