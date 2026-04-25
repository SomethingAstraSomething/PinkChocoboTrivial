try {
    $res = Invoke-WebRequest -Uri "https://github.com/SomethingAstraSomething/PinkChocoboTrivial/releases/latest/download/PinkChocoboTrivial.zip" -Method Head
    Write-Host "STATUS_CODE:" $res.StatusCode
} catch {
    Write-Host "STATUS_CODE: ERROR ($($_.Exception.Message))"
}
