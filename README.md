Esimo Game Client
=================


Description
-----------
Đây là dự án Esimo với các game liên quan như:
+ Esimo Phỏm.
+ Esimo TLMN.
+ Esimo Chắn.
Hỗ trợ các nền tảng: Android, iOS, WebPlayer, PC - Linux or MacOS Standalone
Game được xây dựng trên Game Engine - Unity3d.
Được sử dụng với ngôn ngữ C# cùng các Plugins.


Design document
---------------
Sẽ cập nhật sau (vì chưa có, hoặc có mà chưa biết... :D)


Dependency library
------------------
Unity
-----
- Facebook Unity SDK, for Facebook integration
- NGUI, http://www.tasharen.com/?page_id=140
- Prime31


TroubleShoot
------------
Unity
-----
Nếu sử dụng Unity Serializer trên nền tảng iOS đôi khi bạn sẽ gặp phải vấn đề do cơ chế AOT (IOS: AOT ahead of time). Nó sẽ gây ra crash trên các thiết bị iOS. Bởi mặc định Mono không phân bổ được một số lượng lớn các Type2 trampolines có liên quan đến việc sử dụng các giao diện. Để tránh việc này chúng ta nên sử dụng IEnumerator và for thay vì foreach để tránh phải trampolines do sử dụng generics.