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
if ($needContent1 -or $needContent2) {
  $contentBlock = "`r`n  <ItemGroup>`r`n"
  if ($needContent1) { $contentBlock += "    <Content Include=\"ChatbotService.asmx\" />`r`n" }
  if ($needContent2) { $contentBlock += "    <Content Include=\"Assets\\Js\\chatbot.js\" />`r`n" }
  $contentBlock += "  </ItemGroup>`r`n"
  $append += $contentBlock
}
if ($needCompile1 -or $needCompile2) {
  $compileBlock = "`r`n  <ItemGroup>`r`n"
  if ($needCompile1) { $compileBlock += "    <Compile Include=\"App_Code\\ChatbotLogic.cs\" />`r`n" }
  if ($needCompile2) {
    $compileBlock += "    <Compile Include=\"ChatbotService.asmx.cs\">`r`n"
    $compileBlock += "      <DependentUpon>ChatbotService.asmx</DependentUpon>`r`n"
    $compileBlock += "      <SubType>ASPXCodeBehind</SubType>`r`n"
    $compileBlock += "    </Compile>`r`n"
  }
  $compileBlock += "  </ItemGroup>`r`n"
  $append += $compileBlock
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
if (-not $cfg.configuration.appSettings) {
  $app = $cfg.CreateElement('appSettings')
  # Insert after connectionStrings if present
  if ($cfg.configuration.connectionStrings) {
    [void]$cfg.configuration.InsertAfter($app, $cfg.configuration.connectionStrings)
  } else {
    [void]$cfg.configuration.AppendChild($app)
  }
}

function Ensure-AppSetting {
  param([string]$key, [string]$val)
  $nodes = $cfg.configuration.appSettings.SelectNodes("add[@key='" + $key + "']")
  if ($nodes -and $nodes.Count -gt 0) { return }
  $add = $cfg.CreateElement('add')
  $add.SetAttribute('key', $key)
  $add.SetAttribute('value', $val)
  [void]$cfg.configuration.appSettings.AppendChild($add)
}

Ensure-AppSetting 'OpenAI:Provider' 'OpenAI'
Ensure-AppSetting 'OpenAI:ApiKey' ''
Ensure-AppSetting 'OpenAI:Model' 'gpt-3.5-turbo'
Ensure-AppSetting 'OpenAI:Endpoint' ''
Ensure-AppSetting 'OpenAI:DeploymentId' ''
Ensure-AppSetting 'OpenAI:ApiVersion' '2024-02-01'

$cfg.Save($cfgPath)
Write-Host 'Patched Web.config appSettings.'
