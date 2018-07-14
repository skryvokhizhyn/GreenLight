#include "DBQueryResult.h"

#include <iostream>
#include <sstream>

using namespace glm;

DBQueryResult::DBQueryResult(gsl::not_null<sqlite3*> dbConnection, const std::string & query)
	: m_pDb(dbConnection)
{
	const int rc = sqlite3_prepare_v2(m_pDb, query.c_str(), query.length(), &m_pStmt, nullptr);

	if (rc)
	{
		std::stringstream errMsg;
		errMsg << "Can't prepare sql statement: " << sqlite3_errmsg(m_pDb);
		throw std::runtime_error(errMsg.str());
	}
}

DBQueryResult::~DBQueryResult()
{
	const int ret = sqlite3_finalize(m_pStmt);

	if (ret != SQLITE_OK)
	{
		try
		{
			std::cerr << "Can't prepare sql statement: " << sqlite3_errstr(ret);
		}
		catch (...)
		{
		}
	}
}

bool DBQueryResult::Next()
{
	const int ret = sqlite3_step(m_pStmt);

	switch (ret)
	{
	case SQLITE_DONE:
		return false;
	case SQLITE_ROW:
		return true;
	default:
		{
			std::stringstream errMsg;
			errMsg << "Can't prepare sql statement: " << sqlite3_errstr(ret);
			throw std::runtime_error(errMsg.str());
		}
	}
}

template<>
double DBQueryResult::Get<double>(uint16_t col) const
{
	auto res = sqlite3_column_double(m_pStmt, col);

	const int ret = sqlite3_errcode(m_pDb);

	switch (ret)
	{
	case SQLITE_DONE:
	case SQLITE_ROW:
		return res;
	default:
		{
			std::stringstream errMsg;
			errMsg << "Can't get column " << col << " as double value: " << sqlite3_errstr(ret);
			throw std::runtime_error(errMsg.str());
		}
	};
}

template<>
float DBQueryResult::Get<float>(uint16_t col) const
{
	auto res = sqlite3_column_double(m_pStmt, col);

	const int ret = sqlite3_errcode(m_pDb);

	switch (ret)
	{
	case SQLITE_DONE:
	case SQLITE_ROW:
		return gsl::narrow_cast<float>(res);
	default:
	{
		std::stringstream errMsg;
		errMsg << "Can't get column " << col << " as float value: " << sqlite3_errstr(ret);
		throw std::runtime_error(errMsg.str());
	}
	};
}

template<>
int DBQueryResult::Get<int>(uint16_t col) const
{
	auto res = sqlite3_column_int(m_pStmt, col);

	const int ret = sqlite3_errcode(m_pDb);

	switch (ret)
	{
	case SQLITE_DONE:
	case SQLITE_ROW:
		return res;
	default:
		{
			std::stringstream errMsg;
			errMsg << "Can't get column " << col << " as int value: " << sqlite3_errstr(ret);
			throw std::runtime_error(errMsg.str());
		}
	};
}
