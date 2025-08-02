using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LibraFlow.Models;

namespace LibraFlow.Helpers
{
    public class CsvImportService
    {
        public static List<Book> ParseBooksFromCsv(string filePath)
        {
            var books = new List<Book>();
            var lines = File.ReadAllLines(filePath);
            
            if (lines.Length == 0)
                throw new InvalidOperationException("CSV file is empty.");

            // Skip header row if it exists
            var startIndex = HasBooksHeader(lines[0]) ? 1 : 0;
            
            for (int i = startIndex; i < lines.Length; i++)
            {
                try
                {
                    var book = ParseBookFromCsvLine(lines[i], i + 1);
                    if (book != null)
                        books.Add(book);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error parsing line {i + 1}: {ex.Message}");
                }
            }

            return books;
        }

        public static List<Member> ParseMembersFromCsv(string filePath)
        {
            var members = new List<Member>();
            var lines = File.ReadAllLines(filePath);
            
            if (lines.Length == 0)
                throw new InvalidOperationException("CSV file is empty.");

            // Skip header row if it exists
            var startIndex = HasMembersHeader(lines[0]) ? 1 : 0;
            
            for (int i = startIndex; i < lines.Length; i++)
            {
                try
                {
                    var member = ParseMemberFromCsvLine(lines[i], i + 1);
                    if (member != null)
                        members.Add(member);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error parsing line {i + 1}: {ex.Message}");
                }
            }

            return members;
        }

        private static bool HasBooksHeader(string firstLine)
        {
            var fields = ParseCsvLine(firstLine);
            return fields.Any(f => f.Equals("Title", StringComparison.OrdinalIgnoreCase) ||
                                  f.Equals("Author", StringComparison.OrdinalIgnoreCase) ||
                                  f.Equals("ISBN", StringComparison.OrdinalIgnoreCase));
        }

        private static bool HasMembersHeader(string firstLine)
        {
            var fields = ParseCsvLine(firstLine);
            return fields.Any(f => f.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                                  f.Equals("Email", StringComparison.OrdinalIgnoreCase));
        }

        private static Book ParseBookFromCsvLine(string line, int lineNumber)
        {
            var fields = ParseCsvLine(line);
            
            if (fields.Length < 3)
                throw new InvalidOperationException($"Line {lineNumber} must have at least 3 fields (Title, Author, ISBN).");

            var title = fields[0]?.Trim();
            var author = fields[1]?.Trim();
            var isbn = fields[2]?.Trim();

            if (string.IsNullOrWhiteSpace(title))
                throw new InvalidOperationException($"Title cannot be empty on line {lineNumber}.");
            
            if (string.IsNullOrWhiteSpace(author))
                throw new InvalidOperationException($"Author cannot be empty on line {lineNumber}.");

            return new Book
            {
                Title = title,
                Author = author,
                ISBN = isbn ?? string.Empty,
                IsCheckedOut = false
            };
        }

        private static Member ParseMemberFromCsvLine(string line, int lineNumber)
        {
            var fields = ParseCsvLine(line);
            
            if (fields.Length < 2)
                throw new InvalidOperationException($"Line {lineNumber} must have at least 2 fields (Name, Email).");

            var name = fields[0]?.Trim();
            var email = fields[1]?.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException($"Name cannot be empty on line {lineNumber}.");
            
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException($"Email cannot be empty on line {lineNumber}.");

            return new Member
            {
                Name = name,
                Email = email
            };
        }

        private static string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var inQuotes = false;
            var currentField = string.Empty;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Escaped quote
                        currentField += '"';
                        i++; // Skip next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField);
                    currentField = string.Empty;
                }
                else
                {
                    currentField += c;
                }
            }
            
            fields.Add(currentField);
            return fields.ToArray();
        }

        public static string GetCsvTemplate()
        {
            return "Title,Author,ISBN\n" +
                   "\"Sample Book Title\",\"Sample Author\",\"978-0123456789\"\n" +
                   "\"Another Book\",\"Another Author\",\"978-9876543210\"";
        }

        public static string GetMembersCsvTemplate()
        {
            return "Name,Email\n" +
                   "\"John Doe\",\"john.doe@example.com\"\n" +
                   "\"Jane Smith\",\"jane.smith@example.com\"";
        }
    }
}
