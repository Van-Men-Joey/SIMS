using SIMS.CsvMappings;
using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Repositories.Help_forCsv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace SIMS.Repositories
{
    public class AssignmentRepositoryCsv : BaseCsvRepository<Assignment>, IAssignmentRepository
    {
        public AssignmentRepositoryCsv()
            : base(CsvFilePathProvider.GetPath("assignments.csv"))
        {
        }

        protected override ClassMap<Assignment> GetClassMap()
        {
            return new AssignmentMap();
        }

        public Task<IEnumerable<Assignment>> GetAllAsync()
        {
            var list = ReadAll();
            return Task.FromResult(list.AsEnumerable());
        }

        public Task<Assignment?> GetByIdAsync(string id)
        {
            var assignment = ReadAll().FirstOrDefault(a => a.Id == id);
            return Task.FromResult(assignment);
        }

        public Task<IEnumerable<Assignment>> GetByCourseIdAsync(string courseId)
        {
            var list = ReadAll().Where(a => a.CourseId == courseId).ToList();
            return Task.FromResult(list.AsEnumerable());
        }

        public Task AddAsync(Assignment assignment)
        {
            var list = ReadAll();

            if (list.Any(a => a.Id == assignment.Id))
                throw new Exception("Assignment with same Id already exists");

            list.Add(assignment);
            WriteAll(list);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Assignment assignment)
        {
            var list = ReadAll();
            var idx = list.FindIndex(a => a.Id == assignment.Id);
            if (idx == -1)
                throw new Exception("Assignment not found");

            list[idx] = assignment;
            WriteAll(list);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var list = ReadAll();
            var item = list.FirstOrDefault(a => a.Id == id);
            if (item != null)
            {
                list.Remove(item);
                WriteAll(list);
            }
            return Task.CompletedTask;
        }
    }
}
