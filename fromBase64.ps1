#####################################################################
# 操作ログ復元用スクリプト
#####################################################################
#$file = 'C:\\Users\\t_kita\\workdir\\RealtimeViewer_2022-04-28.log'
$file = $Args[0]
$byte_array = (Get-Content -Path $file) -as [string[]]
$encoding = [System.Text.Encoding]::GetEncoding('utf-8')
$i=1
foreach ($l in $byte_array) {
    $byte = [System.Convert]::FromBase64String($l)
    $txt = $encoding.GetString($byte)
    Write-Host $txt
    $i++
}
