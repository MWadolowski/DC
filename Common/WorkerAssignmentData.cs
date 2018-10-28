using System.Collections.Generic;

namespace Models {
    public class WorkerAssignmentData {
        public WorkerData worker {
            get; set;
        }
        public List<ProductData> orders {
            get; set;
        }

    }
}
