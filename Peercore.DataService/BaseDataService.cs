using Peercore.DataAccess.Common;
using Peercore.DataAccess.Common.Utilties;
using Peercore.Email.Common;
using Peercore.Workflow.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peercore.Email.DataService
{

    public abstract class BaseDataService<T>
    {
        protected DbDataAccessHandle DataAcessService { get; set; }

        private DbSqlAdapter CommonSql { get; set; }

        public bool IsOperationSuccessful { get; protected set; }

        public DbWorkflowScope WorkflowScope
        {
            get
            {
                DbWorkflowScope scope = new DbWorkflowScope(ApplicationService.Instance.ConnectionString, ApplicationService.Instance.DbProvider);
                return scope;
            }
        }

        public BaseDataService()
        {
            this.DataAcessService = new DbDataServiceHandle(ApplicationService.Instance.ConnectionString, ApplicationService.Instance.DbProvider);
            IsOperationSuccessful = true;
            RegisterSql();
        }

        public BaseDataService(DbWorkflowScope scope)
        {
            // this.DataAcessService = new DbTransactionHandle(scope.ConnectionString, ApplicationService.Instance.DbProvider);
            this.DataAcessService = new DbTransactionHandle(scope.Connection, scope.Transaction, ApplicationService.Instance.DbProvider);
            IsOperationSuccessful = true;
            RegisterSql();
        }

        private void RegisterSql()
        {
            this.CommonSql = new DbSqlAdapter("Peercore.Email.DataService.SQL.OrderSQL.xml", ApplicationService.Instance.DbProvider);
        }

        protected void ConcurrencyCheck(string tableName, string keyField, object keyfieldValue, Int64 modifedTime)
        {
            try
            {
                Int64 lastModifiedTime = (Int64)DataAcessService.GetOneValue(CommonSql["GetModifiedTime"].Format(tableName, keyField, keyfieldValue), null);

                if (lastModifiedTime != modifedTime)
                {
                    //("Concurrent update detected. Refresh data and retry.");
                }
            }
            catch
            {
                throw;
            }

        }

        /// <summary>
        /// Validates the string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public virtual string ValidateString(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value.Replace(@"'", @"''").Trim();
            else
                return string.Empty;
        }
    }
}
