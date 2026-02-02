param(
	[Parameter(Mandatory=$true)][string]$VmName,
	[Parameter(Mandatory=$true)][string]$TemplateName,
	[string]$DataStore,
	[string]$Cluster,
	[string]$NetworkName,
	[bool]$PowerOn = $true
)

Write-Output "Rebuild requested for VM=$VmName from Template=$TemplateName"
Write-Output "Datastore=$Datastore Cluster=$Cluster Network=$NetworkName PowerOn=$PowerOn"

# MOCK: simulate steps
Start-Sleep -Seconds 2
Write-Output "Step 1: Checking existing VM..."
Start-Sleep -Seconds 1
Write-Output "Step 2: Removing VM (if exists)..."
Start-Sleep -Seconds 1
Write-Output "Step 3: Deploying from template..."
Start-Sleep -Seconds 2
Write-Output "Step 4: Applying config (network/IIS)..."
Start-Sleep -Seconds 2

# Return 0 = success
exit 0