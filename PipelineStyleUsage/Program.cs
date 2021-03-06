﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSuperior.PipelineStyle.Tutorial
{
	/// <summary>
	/// Cheat sheet: PipelineStyle uses extension method family - 'Do' on Actions & 'To' on Functions.
	/// </summary>
	class Program
	{
		static 
		void 
		Main(
			string[]							args) 
		{
			var rank1 = ExampleConventional("John");
			var rank2 = ExamplePipelineStyle("John");

			var isSame1 = rank1 == rank2;

			var rank3 = ExampleConventional("Jane");
			var rank4 = ExamplePipelineStyle("Jane");
			
			var isSame2 = rank3 == rank4;
		}

		/// <summary>
		/// Given a person's name returns the ranking of the school the person attended.
		/// Written in conventional C# imperative / OO style.
		/// </summary>
		static 
		int
		ExampleConventional(
			string								name)
		{
			var person = 
				Person.GetPeople()
				.FirstOrDefault(p => p.Name == name);

			if(person == null)												
			{
				var msg = String.Format(
									"Person '{0}' not found.",
									name);

				Logger.Log(msg);

				throw new ApplicationException(msg);
			}

			if (person.Email != null)
				person.SendEmail();														

			var school = person.GetSchool();

			return	school != null		
						?	SchoolRankingBoard.GetRank(school)
						:	-1;												// -1, if person did not go to school
		}

		/// <summary>
		/// Given a person's name returns the ranking of the school the person attended.
		/// Written in C# functional Pipeline style
		/// </summary>
		static 
		int
		ExamplePipelineStyle(
			string								name)
		{
			return 
				Person.GetPeople()
				.FirstOrDefault(p => p.Name == name)					// return person, if found
				.DoIsNull(
					() =>												// if person not found
						String.Format(									
									"Person '{0}' not found.",
									name)
						.Do(											// 'Do' pipes the message object
							Logger.Log)									// log function specified implicitly 
						.Do(											// and throw the exception
							message => { throw new ApplicationException(message); } )		
					)													// else returns the person
				.DoIf(
					person => person.Email != null,						// If an email exists, send the email
					person => person.SendEmail())						// 'DoIf' - pipes the same object - Person
				.To(														
					person => person.GetSchool())						// 'To' - returns another object - School
				.ToNotNull(
					SchoolRankingBoard.GetRank,							// Get school rank - specified implicitly
					-1);												// or return -1, if person has no schooling 
		}
	}

	// -----------------------------------------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------------------------------------

	public
	class Person 
	{
			public
			string								Name;

			public
			School								School;

			public 
			string 								Email;

		public 
		void
		SendEmail()
		{
		}

		/// <summary>
		/// May return null if person did not go to school
		/// </summary>
		public 
		School
		GetSchool()
		{	
			return School;
		}

		public 
		static 
		IEnumerable<Person>
		GetPeople()
		{
			return new List<Person> {
						new Person {
								Name = "John",
								Email = "John@hotmail.com",
								School = null, 
						},
						new Person {
								Name = "Jane",
								School = new School {
											Name = "Stanford",
											}, 
						}
					};				
		}
	}

	// -----------------------------------------------------------------------------------------------------------------

	public
	class School
	{
			public
			string								Name;
	}

	// -----------------------------------------------------------------------------------------------------------------

	static 
	public
	class SchoolRankingBoard
	{
		public 
		static
		int 
		GetRank(
			School								school)
		{
			return	school.Name == "Stanford"
						?	1
						:	10;
				
		}
	}

	// -----------------------------------------------------------------------------------------------------------------

	static 
	public
	class Logger
	{
		public 
		static
		void
		Log(
			string								message)
		{
		}
	}

	// -----------------------------------------------------------------------------------------------------------------

}
