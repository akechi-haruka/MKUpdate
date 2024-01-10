using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKUpdate {

    enum RuleType {
        not_folder,
        folder,
        file_contains,
        file_not_contains,
        folder_contains,
        folder_not_contains,
        updated_after,
        created_after
    }

    class ParseRule {
        public RuleType rule;
        public String param;

        public ParseRule(RuleType rule, string param) {
            this.rule = rule;
            this.param = param;
        }
    }

    class Program {

        private static List<ParseRule> rules = new List<ParseRule>();

        static void Main(string[] args) {
            if (args.Length < 3) {
                Console.WriteLine("Usage: MKUpdate <fromdir> <todir> <filetype1,filetype2,...> [rule1,rule2,...]");
                Console.WriteLine("Rules can be:");
                Console.WriteLine("not_folder:example\t\t\texclude all files in the example sub-directory");
                Console.WriteLine("folder:example\t\t\t\texclude all files NOT in the example sub-directory");
                Console.WriteLine("file_contains:example\t\t\texclude all files NOT containing the word example");
                Console.WriteLine("file_not_contains:example\t\texclude all files containing the word example");
                Console.WriteLine("folder_contains:example\t\t\texclude all files NOT containing the word example in their path name");
                Console.WriteLine("folder_not_contains:example\t\texclude all files containing the word example in their path name");
                Console.WriteLine("updated_after:2020-01-01\t\tonly include files edited after 2020-01-01");
                Console.WriteLine("created_after:2020-01-01\t\tonly include files created after 2020-01-01");
                return;
            }
            String from = args[0];
            String to = args[1];
            String[] types = args[2].Split(',');
            if (args.Length > 3) {
                String[] ruleargs = args[3].Split(',');
                foreach (String rulearg in ruleargs) {
                    String[] data = rulearg.Split(':');
                    rules.Add(new ParseRule((RuleType)Enum.Parse(typeof(RuleType), data[0]), data[1]));
                }
            }
            Console.WriteLine("Gathering files...");
            foreach (String type in types) {
                foreach (string file in Recurse(from, type)) {
                    Copy(file, to);
                }
            }
        }

        private static void Copy(string file, string target_dir) {
            if (!Directory.Exists(target_dir)) {
                Directory.CreateDirectory(target_dir);
            }
            var path = target_dir + "/" + Path.GetDirectoryName(file);
            var fn = Path.GetFileName(file);
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            Console.WriteLine(file);
            String target = path + "/" + fn;
            if (File.Exists(target)) {
                File.Delete(target);
            }
            File.Copy(file, target);
        }

        private static IEnumerable<string> Recurse(string path, string file_ext) {
            //Console.WriteLine("Processing: " + path);
            List<String> files = new List<string>();
            if (Directory.Exists(path)) {
                foreach (var subdir in Directory.EnumerateDirectories(path)) {
                    files.AddRange(Recurse(subdir, file_ext));
                }
                foreach (var file in Directory.EnumerateFiles(path)) {
                    if (file.EndsWith(file_ext)) {
                        bool allow = true;
                        foreach (ParseRule r in rules) {
                            if (r.rule == RuleType.not_folder) {
                                if (file.Contains(r.param)) {
                                    allow = false;
                                }
                            } else if (r.rule == RuleType.folder) {
                                if (!file.Contains(r.param)) {
                                    allow = false;
                                }
                            } else if (r.rule == RuleType.file_contains) {
                                String content = File.ReadAllText(file);
                                if (!content.Contains(r.param)) {
                                    allow = false;
                                }
                            } else if (r.rule == RuleType.file_not_contains) {
                                String content = File.ReadAllText(file);
                                if (content.Contains(r.param)) {
                                    allow = false;
                                }
                            } else if (r.rule == RuleType.updated_after) {
                                if (File.GetLastWriteTime(file) < DateTime.Parse(r.param)) {
                                    allow = false;
                                }
                            } else if (r.rule == RuleType.created_after) {
                                if (File.GetCreationTime(file) < DateTime.Parse(r.param)) {
                                    allow = false;
                                }
                            }
                        }
                        if (allow) {
                            files.Add(file);
                        }
                    }
                }
            }
            return files;
        }
    }
}
