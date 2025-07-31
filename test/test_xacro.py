import os
import pytest
import tempfile
import subprocess
import shutil
import xml.etree.ElementTree as ET

from ament_index_python.packages import get_package_share_directory


def check_meshes(urdf_file):
    # Parse URDF file
    root = ET.parse(urdf_file).getroot()
    # Find all mesh elements
    meshes = root.findall(".//mesh")
    # List of all failed meshes [filename: reason]
    failed_meshes = []
    # Check that all meshes exist
    for mesh in meshes:
        filename = mesh.get("filename")
        if filename.startswith("package://"):
            package_name, _, package_path = filename[10:].partition("/")
            package_share_path = get_package_share_directory(package_name)
            if not os.path.exists(f"{package_share_path}/{package_path}"):
                failed_meshes.append(f"{filename}: file does not exist")
        else:
            failed_meshes.append(f"{filename}: not a package:// path")

    assert not failed_meshes, f"Failed meshes:\n{failed_meshes}"


# Get all robot xacro files
xacro_files = []
package_name = 'robotnik_description'
package_share_directory = f"{get_package_share_directory(package_name)}/robots"
for root, dirs, files in os.walk(package_share_directory):
    for file in files:
        if file.endswith('.xacro'):
            xacro_files.append(os.path.join(root, file))

@pytest.mark.parametrize('xacro_file', xacro_files)
def test_xacro_file(xacro_file):
    (fd, tmp_urdf_output_file) = tempfile.mkstemp(suffix=".urdf")
    os.close(fd)

    namespace = "robot"
    frame_prefix = "robot"
    gazebo_ignition = "False"

    xacro_path = shutil.which('xacro')
    assert xacro_path, "xacro is not installed"

    check_urdf_path = shutil.which('check_urdf')
    assert check_urdf_path, "check_urdf is not installed"

    xacro_command = (
        f"{ shutil.which('xacro') }"
        f" { xacro_file }"
        f" namespace:={namespace}"
        f" prefix:={frame_prefix}"
        f" gazebo_ignition:={gazebo_ignition}"
        f" -o { tmp_urdf_output_file }"
    )
    check_command = (
        f"{ shutil.which('check_urdf') }"
        f" { tmp_urdf_output_file }"
    )

    try:
        # Generate URDF file
        print(f"running: {xacro_command}")
        xacro_process = subprocess.run(
            xacro_command, capture_output=True, text=True, shell=True
        )
        assert xacro_process.returncode == 0, f"> xacro failed: {xacro_process.stderr}"
        assert os.path.exists(tmp_urdf_output_file), f"> xacro failed, output file not found: {tmp_urdf_output_file}"

        # Check URDF file
        print(f"running: {check_command}")
        check_process = subprocess.run(
            check_command, capture_output=True, text=True, shell=True
        )
        assert check_process.returncode == 0, f"> check_urdf failed: {check_process.stderr}"

        # Check meshes
        check_meshes(tmp_urdf_output_file)

    finally:
        os.remove(tmp_urdf_output_file)


if __name__ == "__main__":
    test_xacro_file(xacro_files[0])
