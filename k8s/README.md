# Setup cluster

## Create namespaces
```bash
kubectl create namespace sqlserver
kubectl create namespace aspnet-todo
```

# Create secrets
```bash
# Setup SQL Server sa user password
kubectl create secret generic sqlserver-secrets --from-literal=MSSQL_SA_PASSWORD="MyC0m9l&xP@ssw0rd" -n sqlserver

# Setup application connection string
kubectl create secret generic aspnet-todo-secrets --from-literal=connectionString="Persist Security Info=False;User ID=sa;Password=MyC0m9l&xP@ssw0rd;Initial Catalog=TodoList;Server=sqlserver-service.sqlserver.svc.cluster.local;TrustServerCertificate=True" -n aspnet-todo


```