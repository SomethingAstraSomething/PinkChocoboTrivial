$Uri = "https://api.github.com/repos/SomethingAstraSomething/PinkChocoboTrivial/actions/runs"
$response = Invoke-RestMethod -Uri $Uri
$latestRun = $response.workflow_runs | Select-Object -First 1
Write-Host "Latest run status:" $latestRun.conclusion
$jobsUri = $latestRun.jobs_url
$jobsResponse = Invoke-RestMethod -Uri $jobsUri
$job = $jobsResponse.jobs | Select-Object -First 1
Write-Host "Job details:"
$job.steps | Select-Object -Property name, conclusion
