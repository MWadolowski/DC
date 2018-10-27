using System.IO;
using Models;

namespace FirstDecision {
    internal static class Database {
        public static DatabaseTable<Worker> worker;
        public static DatabaseTable<WorkerAssignment> assingments;

        static Database() {
            

            worker = new DatabaseTable<Worker>("worker.json", true);
            assingments = new DatabaseTable<WorkerAssignment>("assignment.json", false);
        }

        /**
         * Do czyszczenia plików jakie powstają w czasie działania
         */
        public static void fileCleanup() {
            File.Delete("worker.json");
            File.Delete("assignment.json");
        }

        /**
         * Temporary - żeby załadować bazę
         */
        public static void start() {
        }
    }
}
