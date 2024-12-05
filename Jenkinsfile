pipeline {
  stage('Clean and checkout'){
    cleanWs()
    checkout scm
  }
    
  stage('Restore'){
    sh 'dotnet restore src/TestProject.sln'
  }
    
  stage('Clean'){
    sh 'dotnet clean src/TestProject.sln'
  }
    
  stage('Build'){
    sh 'dotnet build src/TestProject.sln --configuration Release'
  }
  stage('SonarQube Analysis') {
    def scannerHome = tool 'SonarScanner for MSBuild'
    withSonarQubeEnv() {
      sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll begin /k:\"articleservice\""
      sh "dotnet build"
      sh "dotnet ${scannerHome}/SonarScanner.MSBuild.dll end"
    }
  }
}
