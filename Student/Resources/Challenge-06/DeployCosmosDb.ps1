$resourceGroup="<Add the resource group name>"
$location="<Add the location>"
$cosmosDbAccountName="<Add the Cosmos DB account name>"
$cosmosDbDatabaseName="<Add the Cosmos DB database name>"
$cosmosDbContainer="<Add the Cosmos DB container name>"

# Log in to Azure
# az login

# Create a resource group
az group create --name $resourceGroup --location $location

# Create a Cosmos DB account
az cosmosdb create --name $cosmosDbAccountName --resource-group $resourceGroup --locations regionName=$location

# Create a database
az cosmosdb sql database create --account-name $cosmosDbAccountName --resource-group $resourceGroup --name $cosmosDbDatabaseName

# Create a container
az cosmosdb sql container create --account-name $cosmosDbAccountName --resource-group $resourceGroup --database-name $cosmosDbDatabaseName --name $cosmosDbContainer --partition-key-path /id

# Enable the NoSQL Vector Search capability
az cosmosdb update --resource-group $resourceGroup --name $cosmosDbAccountName  --capabilities EnableNoSQLVectorSearch