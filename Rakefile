#!/usr/bin/env ruby

require 'tools/rake/tasks'

ROOT = File.dirname(__FILE__)
COMPANY = 'BRFKredit a/s'
PRODUCT_NS = 'Maxfire'
PRODUCT_DESC = 'Skatteberegning Library'
PRODUCT_VERSION = '0.1'
COPYRIGHT = 'Copyright 2010 Morten Maxild. All rights reserved.';
CONFIGURATION = 'Debug'
SOLUTION = 'Skat.sln'

# 3rd party program paths
PATHS = {
	:ncover => "#{File.join(Rake::OSArchitecture.programfiles, 'NCover')}"
}

ARCHIVE = {
	:root => 'archive',
	:build => 'archive/build',
	:build_output => 'archive/build/' + CONFIGURATION, # TODO: Copy to build output folder 
	:results => 'archive/results'
}
ARCHIVE.each { |k, v| ARCHIVE[k] = File.join(ROOT, v) }

PROP = {
	:version => PRODUCT_VERSION
}

def rf(filename)
	File.join(ARCHIVE[:results], filename)
end

def publish_tool(name, exclude_pattern=nil)
	dest = File.join(ARCHIVE[:tools], name)
	Dir.mkdir dest unless File.exists? dest
	Rake::TaskUtils::cp_wo_svn(File.join(ROOT, 'tools', name), dest, exclude_pattern)
end

namespace :build do

	dev_build = [
		:clean_build, 
		:compile, 
		:fxcop, 
		:simian, 
		:run_tests, 
		:copy_output_assemblies
	]
	
	desc "Before Commit Build"
	task :dev => dev_build
			
	# create archive folders
	task :init do
		Rake::TaskUtils::flash "creating archive folders"
		ARCHIVE.each { |k, v| mkdir_p v unless File.exist?(v) }
	end
	
	# remove archive folders
	task :clean_all do
		Rake::TaskUtils::flash "Removing all archive folders"
		remove_dir ARCHIVE[:root] if File.exist?(ARCHIVE[:root])
	end
	
	# remove archive folders, except packages and latestpackage
	task :clean_build do
		Rake::TaskUtils::flash "Removing build archive folders"
		ARCHIVE.each { |k, v| remove_dir v unless !File.exists?(v) || k == :root || k == :latestpackage || k == :packages }
	end
	
	# Generate CommonAssemblyInfo.cs file
	Rake::AssemblyInfoTask.new(:version) do |asminfo|
		asminfo.company = COMPANY
		asminfo.configuration = CONFIGURATION
		asminfo.copyright = COPYRIGHT
		asminfo.product = PRODUCT_DESC
		build_number = ENV["CCNetLabel"].nil? ? '0' : ENV["CCNetLabel"].to_s
		revision_number = File.exists?('.svn') ? Rake::TaskUtils::svn_revision : '0'
		PROP[:version] = "#{PRODUCT_VERSION}.#{build_number}.#{revision_number}"
		asminfo.version = Rake::Version.new(PROP[:version])
	end
	
	desc "Compile all code"
	Rake::MsBuildTask.new(:compile => [:init, :version]) do |msbuild|
		msbuild.tools_version = '4.0'
		msbuild.target_framework_version = 'v4.0'
		msbuild.project = File.join('src', SOLUTION)
		msbuild.targets << 'Clean'
		msbuild.targets << 'Build'
		msbuild.verbosity_level = 'minimal'
		msbuild.properties['Configuration'] = CONFIGURATION
		msbuild.properties['TreatWarningsAsErrors'] = "true"
		msbuild.properties['MvcBuildViews'] = "true"
		msbuild.properties['MvcPublishWebsite'] = "false"
	end
	
	desc "Perform static code analysis"
	Rake::FxCopTask.new(:fxcop => :compile) do |fxcop|
		fxcop.tool_path = File.join(ROOT, 'tools/fxcop')
		fxcop.rich_console_output = false
		fxcop.assemblies = FileList["src/app/**/bin/#{CONFIGURATION}/#{PRODUCT_NS}.*.dll"]
		fxcop.results_file = rf('FxCopReport.xml')
		fxcop.assembly_search_path = "#{ARCHIVE[:build_output]}"
	end
	
	desc "Run similarity analyser"
	Rake::SimianTask.new(:simian => :init) do |simian|
		simian_path = File.join(ROOT, 'tools', 'simian')
		simian.tool_path = File.join(simian_path, 'bin')
		simian.results_file = rf('SimianReport.xml')
		simian.stylesheet = File.join(simian_path, 'simian.xsl')
	end
	
	test_task_names = []
	coverage_results_filenames = []
	build_assemblies = []
	
	FileList["src/test/**/bin/#{CONFIGURATION}/#{PRODUCT_NS}.*.UnitTests.dll"].each do |test_assembly|
		# get the name of the assembly without extension (e.g. Maxfire.Web.Mvc.UnitTests)
		test_assembly_name =  File.basename(test_assembly).ext
		# Find the corresponding production assembly (e.g. Maxfire.Web.Mvc.dll)
		build_assemblies << test_assembly.slice(0, test_assembly.size - '.UnitTests.dll'.size) + '.dll'
		# slice away the Maxfire. prefix and the .UnitTests postfix.
		name = test_assembly_name.slice(PRODUCT_NS.size + 1, test_assembly_name.size - (PRODUCT_NS.size + 1 + '.UnitTests'.size))
		# remove any dots
		name.gsub!(/[\.]/,'')
		# Create the dynamic task name (run_WebMvc_tests)
		task_name = "run_#{name}_tests"
		test_task_names << task_name
		desc "Run tests in #{test_assembly_name} with coverage"
		Rake::XUnitTask.new(task_name => :compile) do |xunit|
			xunit.clr_version = '4'
			xunit.xunit_path = File.join(ROOT, 'tools', 'xunit')
			xunit.test_assembly = test_assembly
			xunit.results_folder = ARCHIVE[:results]
			xunit.test_results_filename = "#{name}UnitTestResults.xml"
			xunit.test_stylesheet = File.join(ROOT, 'tools', 'xunit', 'xUnitSummary.xsl')
			xunit.calculate_coverage = true
			xunit.coverage_exclude_attrs << 'Maxfire.TestCommons.NoCoverageAttribute'
			xunit.ncover_path = PATHS[:ncover]
			coverage_results_filenames << xunit.coverage_results_filename = "#{name}UnitTestCoverage.xml"
			xunit.coverage_log_filename = "#{name}UnitTestCoverage.log"
			xunit.coverage_assemblies = FileList["#{File.dirname(test_assembly)}/#{PRODUCT_NS}*.dll"].exclude(/.*Tests.dll$/)
		end
	end
	
	desc "Run all the unit tests"
	task :run_tests => test_task_names
	
	# This 'hacky' technique of copying production assemblies to the build output folder
	# relies on 'run unit tests' task. Each unit test project has to be setup satisfying
	# the following rules:
	#   1) A reference to xunit.dll
	#   2) A reference to the corresponding production assembly under test (i.e A.UnitTests.dll references A.dll).
	task :copy_output_assemblies => :run_tests do
		build_assemblies.each do |src| 
			cp src, ARCHIVE[:build_output] 
			cp src.ext('pdb'), ARCHIVE[:build_output] 
		end
	end
	
end

namespace :util do

	desc "Force NCover and xUnit.net to run under WOW64 (x86 emulator that allows 32-bit Windows applications to run on 64-bit Windows"
	task :ncover64 do
		ncover_path = Rake::TaskUtils.to_windows_path(File.join(ROOT, 'tools', 'ncover', 'NCover.Console.exe'))
		xunit_path = Rake::TaskUtils.to_windows_path(File.join(ROOT, 'tools', 'xunit', 'xunit.console.exe'))
		working_dir = File.join("#{ENV['ProgramW6432']}", 'Microsoft SDKs', 'Windows', 'v7.0', 'bin')
		cd working_dir do
			sh "CorFlags.exe #{ncover_path} /32BIT+"
			sh "CorFlags.exe #{xunit_path} /32BIT+"
		end
	end
	
	desc "Start Visual Studio"
	task :ide do
		working_folder = File.join(ROOT, 'src')
		cd working_folder do
			sh SOLUTION
		end	
	end
	
	desc "Open Windows Explorer with focus on totalberegner folder"
	task :explorer do
		sh 'start explorer /e,c:\dev\projects,/Select,c:\dev\projects\maxfire'
	end
			
end