using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Interfaces;
using Domain.Models;
using Domain.ViewModels;
using Domain.ViewModels.Request;
using Storage.Interfaces;

namespace BusinessLogic.Services
{
    public class ProblemResourceTaskService : BaseCrudService<ProblemResourceTask>, IProblemResourceTaskService
    {
        public ProblemResourceTaskService(IRepository<ProblemResourceTask> repository) : base(repository)
        {
        }

        public List<ProblemResourceTask> GetResourceTasks(ProblemResource resource)
        {
            return GetAll().Where(x => x.Resource == resource).ToList();
        }

        public bool IsTaskExist(string name, ProblemResource resource)
        {
            var task = GetAll().FirstOrDefault(x =>
                x.Resource == resource &&
                x.Title == name);

            if (task == null)
                return false;

            return true;
        }

        public ProblemResourceTask CreateTask(CreateUpdateProblemResourceTask request, ProblemResource resource)
        {
            if (request.ID != 0)
                return null;

            if (IsTaskExist(request.Title, resource))
                return null;

            var entity = new ProblemResourceTask
            {
                Resource = resource,
                Title = request.Title,
                IsDone = request.IsDone
            };

            Create(entity);

            return entity;
        }

        public ProblemResourceTask UpdateTask(CreateUpdateProblemResourceTask request)
        {
            if (request.ID == 0)
                return null;

            var entity = Get(request.ID);
            if (entity == null)
                return null;

            entity.IsDone = request.IsDone;
            Update(entity);

            return entity;
        }

        public ProblemResourceTask CreateUpdateTask(CreateUpdateProblemResourceTask request, ProblemResource resource = null)
        {
            if (request.ID == 0)
                return CreateTask(request, resource);

            return UpdateTask(request);
        }
    }
}
