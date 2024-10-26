class StringUtils:
    """
    StringUtils 工具类，提供与字符串处理相关的静态方法。
    """

    @staticmethod
    def snake_to_camel_uppercase(snake_str: str) -> str:
        """
        将下划线格式的字符串转换为首字母大写的驼峰形式。

        :param snake_str: 以下划线分隔的字符串，例如 "example_string_format"
        :return: 转换后的驼峰形式字符串，例如 "ExampleStringFormat"
        """
        # 使用 split 方法将字符串按下划线分隔成列表
        parts = snake_str.split('_')
        # 对列表中的每个单词使用 str.capitalize() 方法进行首字母大写
        camel_case_str = ''.join(part.capitalize() for part in parts)
        return camel_case_str