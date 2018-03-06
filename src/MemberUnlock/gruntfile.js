module.exports = function (grunt) {
    grunt.initConfig({

        // Clean the Site project /App_Plugins/MemberUnlock folder
        clean: {
            folder: ['../MemberUnlock.Site/App_Plugins/MemberUnlock'],
            options: { force: true }
        },

        // Create /App_Plugins/MemberUnlock folder
        mkdir: {
            all: {
                options: {
                    create: ['../MemberUnlock.Site/App_Plugins/MemberUnlock']
                }
            }
        },

        // Copy files to the /App_Plugins/MemberUnlock folder
        copy: {
            files: {
                cwd: 'Web/UI/App_plugins/MemberUnlock',
                src: '**/*',
                dest: '../MemberUnlock.Site/App_Plugins/MemberUnlock',
                expand: true
            }
        },

        // Watch when files are updated => copy to the Site project /App_Plugins/MemberUnlock folder
        watch: {
            memberlogin: {
                files: ['Web/UI/App_plugins/MemberUnlock/**/*'],
                tasks: ['copy']
            }
        }

    });


    // Load NPM tasks
    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-mkdir');
};