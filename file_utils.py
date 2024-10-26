import json
import csv
import re
import os
import pandas as pd

class FileUtils:
    """
    文件IO工具类
    """

    @staticmethod
    def write_to_json_file(content: dict, filename: str):
        # 将字典转换为 JSON 并写入到文件中
        try:
            with open(filename, 'w', encoding='utf-8') as file:
                json.dump(content, file, indent=4, ensure_ascii=False)
                print(f"JSON 文件已成功创建: {filename}")
        except IOError as e:
            print(f"JSON 文件写入时出错: {e}")

    @staticmethod
    def write_to_file(content: str, filename: str):
        try:
            # 使用 'w' 模式写入文件，确保文件内容按模板格式输出
            with open(filename, 'w', encoding='utf-8') as file:
                file.write(content)
                print(f"文件已成功创建: {filename}")
        except IOError as e:
            print(f"写入文件时出错: {e}")

    @staticmethod
    def append_to_csv_file(content: list, file_path: str):
        try:
            # 打开 TSV 文件并以追加模式写入
            with open(file_path, mode='a', newline='', encoding='utf-8') as file:
                writer = csv.writer(file, delimiter='\t')
                # 遍历数据并逐行写入
                for row in content:
                    writer.writerow(row)

            return True
        except IOError as e:
            print(f"写入csv文件时出错: {e}")
            return False
    
    @staticmethod
    def insert_before_last_brace(content_to_add: str, file_path: str):
        """
        在文件中的最后一个 "}" 前插入指定内容。

        :param file_path: 要操作的文件的路径。
        :param content_to_add: 要插入的内容。
        """
        try:
            # 读取文件内容
            with open(file_path, 'r', encoding='utf-8') as file:
                file_content = file.read()

            # 定位最后一个 "}" 并在其前面插入内容
            updated_content = re.sub(r'(\n}\s*)$', f"{content_to_add}\\1", file_content, flags=re.MULTILINE)

            # 写入更新后的内容回到文件中
            with open(file_path, 'w', encoding='utf-8') as file:
                file.write(updated_content)
        except FileNotFoundError:
            print(f"错误: 文件 {file_path} 未找到。")
        except Exception as e:
            print(f"发生错误: {str(e)}")
    
    @staticmethod
    def read_recipes_tsv() -> pd.DataFrame:
        """
        读取当前目录下的 recipes.tsv 文件。

        :return: 一个包含 TSV 文件内容的 Pandas DataFrame。
        """
        # 获取当前目录下的文件路径
        file_path = os.path.join(os.getcwd(), 'recipes.tsv')
        # 使用 Pandas 读取 TSV 文件
        try:
            df = pd.read_csv(file_path, sep='\t')
            return df
        except FileNotFoundError:
            print(f"文件 {file_path} 未找到。")
            return pd.DataFrame()
        except Exception as e:
            print(f"读取文件时发生错误: {e}")
            return pd.DataFrame()
