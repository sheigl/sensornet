import java.text.SimpleDateFormat

node {
    def app
    def dateFormat = new SimpleDateFormat("yyyyMMdd")
    def date = new Date()
    def buildNumber = "${dateFormat.format(date)}.${env.BUILD_NUMBER}"
    def remote = [:]
    def packageName = "sensornet.server"
    remote.name = "1and1"
    remote.host = "beta.calendarlog.net"
    remote.allowAnyHosts = true

    stage('Clone repository') {
        checkout scm
    }

    withCredentials([(usernamePassword(credentialsId: '862ea950-534e-4434-8579-90f5e4901df8', passwordVariable: 'password', usernameVariable: 'userName'))]) {
        stage('Build image') {
                def dockerfile = '${packageName}.dockerfile'
                app = docker.build("sheigl/pvt:${packageName}-${buildNumber}", "--build-arg SQL_USERNAME=${userName} --build-arg SQL_PASSWORD=${password} -f ${dockerfile} .")
            }
    }
    /* stage('Test image') {
        app.inside {
            sh 'echo "Tests passed"'
        }
    } */

    stage('Push image') {
        docker.withRegistry('https://registry.hub.docker.com', 'DockerHub') {
            app.push("${packageName}-${buildNumber}")
            app.push("${packageName}-latest")
        }
    }

    withCredentials([usernamePassword(credentialsId: '19fa557a-cdeb-47f4-9e05-6b21d10b829a', passwordVariable: 'password', usernameVariable: 'userName')]) {
        remote.user = userName
        remote.password = password

        withCredentials([usernamePassword(credentialsId: 'DockerHub', passwordVariable: 'dockerPassword', usernameVariable: 'dockerUsername')]) {
            stage('Pull image') {
                sshCommand remote: remote, command: "sudo docker login -u ${dockerUsername} -p ${dockerPassword}"
                sshCommand remote: remote, command: "sudo docker pull sheigl/pvt:${packageName}-latest"
            }

            stage('Start image') {
                sshCommand remote: remote, command: "sudo docker stop ${packageName} && sudo docker rm ${packageName}", failOnError: false
                sshCommand remote: remote, command: "sudo docker run -p 9999:80 --name ${packageName} --network ${packageName} -d sheigl/pvt:${packageName}-latest"
            }
        }        
    }
}