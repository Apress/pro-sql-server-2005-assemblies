 // Event categories
//
//  Values are 32 bit values layed out as follows:
//
//   3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
//   1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
//  +---+-+-+-----------------------+-------------------------------+
//  |Sev|C|R|     Facility          |               Code            |
//  +---+-+-+-----------------------+-------------------------------+
//
//  where
//
//      Sev - is the severity code
//
//          00 - Success
//          01 - Informational
//          10 - Warning
//          11 - Error
//
//      C - is the Customer code flag
//
//      R - is a reserved bit
//
//      Facility - is the facility code
//
//      Code - is the facility's status code
//
//
// Define the facility codes
//


//
// Define the severity codes
//


//
// MessageId: 0x00000001L (No symbolic name defined)
//
// MessageText:
//
//  Execution completed
//


//
// MessageId: 0x00000002L (No symbolic name defined)
//
// MessageText:
//
//  Execution failed
//


 // Event messages
//
// MessageId: 0x000003E8L (No symbolic name defined)
//
// MessageText:
//
//  Successfully retrieved data for employee %1%rFor more information, please refer to the website%rhttp://www.JulianSkinner.com/books/SqlAssemblies/ErrorMessages.aspx?id=3000
//


//
// MessageId: 0x000007D0L (No symbolic name defined)
//
// MessageText:
//
//  No rows found for employee %1%rFor more information, please refer to the website%rhttp://www.JulianSkinner.com/books/SqlAssemblies/ErrorMessages.aspx?id=2000
//


//
// MessageId: 0x00000BB8L (No symbolic name defined)
//
// MessageText:
//
//  SQL errors occurred executing stored procedure:%r%1%rFor more information, please refer to the website%rhttp://www.JulianSkinner.com/books/SqlAssemblies/ErrorMessages.aspx?id=3000
//


//
// MessageId: 0x00000FA0L (No symbolic name defined)
//
// MessageText:
//
//  An unknown error occurred executing stored procedure:%r%1%rFor more information, please refer to the website%rhttp://www.JulianSkinner.com/books/SqlAssemblies/ErrorMessages.aspx?id=4000
//


