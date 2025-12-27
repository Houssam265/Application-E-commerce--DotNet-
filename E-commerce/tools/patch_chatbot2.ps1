$ErrorActionPreference = 'Stop'
$projPath = 'c:\Users\houss\source\repos\Application_DOTNET\E-commerce\E-commerce.csproj'
$cfgPath  = 'c:\Users\houss\source\repos\Application_DOTNET\E-commerce\Web.config'

# Load project text
$projText = Get-Content -Raw -LiteralPath $projPath

$needContent1 = ($projText -notmatch 'Content Include="ChatbotService\.asmx"')
$needContent2 = ($projText -notmatch 'Content Include="Assets\\Js\\chatbot\.js"')
$needCompile1 = ($projText -notmatch 'Compile Include="App_Code\\ChatbotLogic\.cs"')
$needCompile2 = ($projText -notmatch 'Compile Include="ChatbotService\.asmx\.cs"')

$append = ''

# Build Content block
$contentLines = @()
if ($needContent1) { $contentLines += '    <Content Include="ChatbotService.asmx" />' }
if ($needContent2) { $contentLines += '    <Content Include="Assets\Js\chatbot.js" />' }
if ($contentLines.Count -gt 0) {
  $append += "`r`n  <ItemGroup>`r`n" + ($contentLines -join "`r`n") + "`r`n  </ItemGroup>`r`n"
}

# Build Compile block
$compileLines = @()
if ($needCompile1) { $compileLines += '    <Compile Include="App_Code\ChatbotLogic.cs" />' }
if ($needCompile2) {
  $compileLines += @'
    <Compile Include="ChatbotService.asmx.cs">
      <DependentUpon>ChatbotService.asmx</DependentUpon>
      <SubType>ASPXCodeBehind</SubType>
    </Compile>
'@
}
if ($compileLines.Count -gt 0) {
  $append += "`r`n  <ItemGroup>`r`n" + ($compileLines -join "`r`n") + "`r`n  </ItemGroup>`r`n"
}

if ($append.Length -gt 0) {
  $idx = $projText.LastIndexOf('</Project>')
  if ($idx -lt 0) { throw 'Unable to find </Project> end tag in project file.' }
  $projText = $projText.Insert($idx, $append)
  Set-Content -LiteralPath $projPath -Value $projText -Encoding UTF8
  Write-Host 'Updated E-commerce.csproj with chatbot entries.'
} else {
  Write-Host 'E-commerce.csproj already contains required chatbot entries.'
}

# Patch Web.config appSettings
[xml]$cfg = Get-Content -LiteralPath $cfgPath
# Switch to text-based patch to avoid XML library quirks in PS
$cfgText = Get-Content -Raw -LiteralPath $cfgPath

if ($cfgText -notmatch '<appSettings>') {
  $appBlock = @"
  <appSettings>
    <add key="OpenAI:Provider" value="OpenAI" />
    <add key="OpenAI:ApiKey" value="" />
    <add key="OpenAI:Model" value="gpt-3.5-turbo" />
    <add key="OpenAI:Endpoint" value="" />
    <add key="OpenAI:DeploymentId" value="" />
    <add key="OpenAI:ApiVersion" value="2024-02-01" />
  </appSettings>
"@
  $cfgText = [regex]::Replace($cfgText, '</configuration>\s*$', ($appBlock + "`r`n</configuration>"), 'Singleline')
} else {
  $insertLines = @()
  if ($cfgText -notmatch 'key="OpenAI:Provider"')    { $insertLines += '    <add key="OpenAI:Provider" value="OpenAI" />' }
  if ($cfgText -notmatch 'key="OpenAI:ApiKey"')      { $insertLines += '    <add key="OpenAI:ApiKey" value="" />' }
  if ($cfgText -notmatch 'key="OpenAI:Model"')       { $insertLines += '    <add key="OpenAI:Model" value="gpt-3.5-turbo" />' }
  if ($cfgText -notmatch 'key="OpenAI:Endpoint"')    { $insertLines += '    <add key="OpenAI:Endpoint" value="" />' }
  if ($cfgText -notmatch 'key="OpenAI:DeploymentId"'){ $insertLines += '    <add key="OpenAI:DeploymentId" value="" />' }
  if ($cfgText -notmatch 'key="OpenAI:ApiVersion"')  { $insertLines += '    <add key="OpenAI:ApiVersion" value="2024-02-01" />' }
  if ($insertLines.Count -gt 0) {
    $insertion = ("`r`n" + ($insertLines -join "`r`n") + "`r`n  ")
    $cfgText = [regex]::Replace($cfgText, '</appSettings>', ($insertion + '</appSettings>'))
  }
}

Set-Content -LiteralPath $cfgPath -Value $cfgText -Encoding UTF8
Write-Host 'Patched Web.config (text mode).' 
