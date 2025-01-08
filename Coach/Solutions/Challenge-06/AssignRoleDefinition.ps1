$resourceGroup="<Add resources group>"
$cosmosDbAccountName="<Add CosmosDB account name>"
$principalId="<Add your prinipal id>"
# Built In role definition "Cosmos DB Built-in Data Reader"
$roleDefinitionId="00000000-0000-0000-0000-000000000002"

# Assign the role definition "Cosmos DB Built-in Data Reader" with id ""00000000-0000-0000-0000-000000000002"" to the principal
# Reference https://learn.microsoft.com/en-us/azure/cosmos-db/how-to-setup-rbac#built-in-role-definitions
az cosmosdb sql role assignment create --account-name $cosmosDbAccountName --resource-group $resourceGroup --scope "/" --principal-id $principalId --role-definition-id $roleDefinitionId 