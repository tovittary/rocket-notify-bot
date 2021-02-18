$pathToExecutable = "RocketNotifyBot.exe";
$serviceName = "RocketNotifyBot";
$serviceDescription = "Rocket.Chat messages notification service";

$service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue;
if ($service) {
    if ($service.Status -eq 'Running') {
        Write-Output "Stopping service: $serviceName";
        Stop-Service $serviceName;
    }
} else {
    New-Service -Name $serviceName -Description $serviceDescription -BinaryPathName $pathToExecutable;
}

Start-Service $serviceName;