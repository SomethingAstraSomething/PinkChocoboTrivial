$response = Invoke-RestMethod -Uri "https://api.github.com/repos/SomethingAstraSomething/PinkChocoboTrivial/actions/runs"
$latestRun = $response.workflow_runs | Select-Object -First 1
$jobsUri = $latestRun.jobs_url
$jobsResponse = Invoke-RestMethod -Uri $jobsUri
$jobId = $jobsResponse.jobs[0].id
$logUri = "https://api.github.com/repos/SomethingAstraSomething/PinkChocoboTrivial/actions/jobs/$jobId/logs"
try {
    $logResponse = Invoke-WebRequest -Uri $logUri -Headers @{"Accept"="application/vnd.github.v3+json"}
    Write-Host $logResponse.Content
} catch {
    Write-Host "Error fetching logs:" $_.Exception.Message
}
