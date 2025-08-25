cmake_minimum_required(VERSION 3.5)
project(robotnik_description)

find_package(ament_cmake REQUIRED)
ament_package_xml()

install(
    DIRECTORY launch meshes urdf robots simulators
    DESTINATION share/${PROJECT_NAME})

ament_export_dependencies(${THIS_PACKAGE_INCLUDE_DEPENDS})

set(_hook "${CMAKE_CURRENT_BINARY_DIR}/${PROJECT_NAME}.dsv")
file(WRITE "${_hook}"
"prepend-non-duplicate;GZ_SIM_RESOURCE_PATH;share/${PROJECT_NAME}/..\n")
install(FILES "${_hook}" DESTINATION share/${PROJECT_NAME})
ament_environment_hooks("${_hook}")

if(BUILD_TESTING)
  find_package(ament_cmake_pytest REQUIRED)
  ament_add_pytest_test(urdf_xacro test/test_xacro.py)
endif()

ament_package()
