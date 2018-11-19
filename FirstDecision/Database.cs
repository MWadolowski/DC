using System.IO;
using Models;

namespace FirstDecision {
    internal static class Database {
        public static DatabaseTable<WorkerData> worker;
        public static DatabaseTable<WorkerAssignmentData> assignments;
        public static DatabaseTable<OrderData> orders;

        static Database() {
            worker = new DatabaseTable<WorkerData>("worker.json");
            assignments = new DatabaseTable<WorkerAssignmentData>("assignment.json");
            orders = new DatabaseTable<OrderData>("order.json");

            if (worker.SelectAll().Count == 0) {
                worker.InsertElement(new WorkerData() { Email = "michwadol@gmail.com", FirstName = "Tytus", LastName = "Bomba" });
                worker.InsertElement(new WorkerData() { Email = "rozanskiwojciech93@gmail.com", FirstName = "Alfons", LastName = "Hipstler" });
                worker.InsertElement(new WorkerData() { Email = "michal.dominik.brach@gmail.com", FirstName = "Mnichas", LastName = "Brach" });
            }
        }

        /**
         * Do czyszczenia plików jakie powstają w czasie działania
         */
        public static void FileCleanup() {
            File.Delete("worker.json");
            File.Delete("assignment.json");
            File.Delete("order.json");
        }

        /**
         * Temporary - żeby załadować bazę
         */
        public static void start() {
        }
    }
}
