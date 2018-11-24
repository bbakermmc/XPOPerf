using System.ComponentModel.DataAnnotations;

namespace ORMBenchmark.Models.EFCore {
    public partial class EFCoreEntity {
        [Key]
        public long Id { get; set; }
        public long Value { get; set; }
    }
}
