pipeline {
  stage('Clean and checkout'){
    steps{
      cleanWs()
      checkout scm
    }
  }
    
  stage('Restore'){
    steps{
      sh 'dotnet restore src/TestProject.sln'
    }
  }
    
  stage('Clean'){
    steps{
      sh 'dotnet clean src/TestProject.sln'
    }
  }
    
  stage('Build'){
    steps{
      sh 'dotnet build src/TestProject.sln --configuration Release'
    }
  }
}
