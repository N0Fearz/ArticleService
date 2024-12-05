pipeline {
  agent any
  tools {
    dotnetsdk 'SDK 8'
  }
  environment {
      DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = "true"
      PATH = "${env.HOME}/.dotnet/tools:${env.PATH}" // Voeg ~/.dotnet/tools toe aan het PATH
  }
  stages{
    stage('Clean and checkout'){
      steps{
        cleanWs()
        checkout scm
      }
    }
      
    stage('Restore'){
      steps{
        sh 'dotnet restore ArticleService.sln'
      }
    }
      
    stage('Clean'){
      steps{
        sh 'dotnet clean ArticleService.sln'
      }
    }
      
    stage('Build'){
      steps{
        sh 'dotnet build ArticleService.sln --configuration Release'
      }
    }
    stage('Install SonarScanner') {
      steps {
          sh 'dotnet tool install --global dotnet-sonarscanner'
          sh 'export PATH="$PATH:~/.dotnet/tools"' // Zorg ervoor dat de tool in het PATH staat
        }
    }
    stage('SonarQube Analysis') {
        steps {
          withSonarQubeEnv('SonarQube') { // Naam van de SonarQube server zoals ingesteld in Jenkins
            sh '''
            dotnet sonarscanner begin /k:"articleservice"
            dotnet build --configuration Release
            dotnet sonarscanner end
            '''
        }
      }
    }
  }
}
